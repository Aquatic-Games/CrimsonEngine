using System.Runtime.CompilerServices;
using Crimson.Math;

namespace Crimson.Graphics;

public sealed class DDS
{
    private const uint Magic = 0x20534444;

    public readonly Size<int> Size;

    public readonly Graphics.PixelFormat Format;

    public readonly uint ArraySize;

    public readonly uint MipLevels;
    
    public readonly Bitmap[,] Bitmaps;

    public unsafe DDS(string path)
    {
        using FileStream stream = File.OpenRead(path);
        using BinaryReader reader = new BinaryReader(stream);

        if (reader.ReadUInt32() != Magic)
            throw new InvalidDataException("File is missing the Magic header. File is malformed or is not a DDS file.");
        
        Header header = new Header();
        reader.Read(new Span<byte>(Unsafe.AsPointer(ref header), sizeof(Header)));

        ref readonly PixelFormat format = ref header.PixelFormat;

        switch (format.FourCC)
        {
            case FourCC.DX10:
            {
                HeaderDX10 dx10Header = new HeaderDX10();
                reader.Read(new Span<byte>(Unsafe.AsPointer(ref dx10Header), sizeof(HeaderDX10)));

                Format = dx10Header.Format switch
                {
                    DxgiFormat.R8G8B8A8Unorm => Graphics.PixelFormat.RGBA8,
                    DxgiFormat.B8G8R8A8Unorm => Graphics.PixelFormat.BGRA8,
                    DxgiFormat.Bc1Unorm => Graphics.PixelFormat.BC1,
                    DxgiFormat.Bc1UnormSrgb => Graphics.PixelFormat.BC1Srgb,
                    DxgiFormat.Bc2Unorm => Graphics.PixelFormat.BC2,
                    DxgiFormat.Bc2UnormSrgb => Graphics.PixelFormat.BC2Srgb,
                    DxgiFormat.Bc3Unorm => Graphics.PixelFormat.BC3,
                    DxgiFormat.Bc4Unorm => Graphics.PixelFormat.BC4U,
                    DxgiFormat.Bc5Unorm => Graphics.PixelFormat.BC5U,
                    DxgiFormat.Bc6HUf16 => Graphics.PixelFormat.BC6U,
                    DxgiFormat.Bc6HSf16 => Graphics.PixelFormat.BC6S,
                    DxgiFormat.Bc7Unorm => Graphics.PixelFormat.BC7,
                    DxgiFormat.Bc7UnormSrgb => Graphics.PixelFormat.BC7Srgb,
                    _ => throw new NotImplementedException()
                };

                break;
            }
            
            case FourCC.DXT1:
                Format = Graphics.PixelFormat.BC1;
                break;
            
            case FourCC.DXT3:
                Format = Graphics.PixelFormat.BC2;
                break;
            
            case FourCC.DXT5:
                Format = Graphics.PixelFormat.BC3;
                break;
            
            case FourCC.BC4U:
                Format = Graphics.PixelFormat.BC4U;
                break;
            
            case FourCC.BC5U:
                Format = Graphics.PixelFormat.BC5U;
                break;
            
            case FourCC.None:
            {
                bool IsPixelFormat(uint rgbBitCount, uint rBitMask, uint gBitMask, uint bBitMask, uint aBitMask)
                {
                    ref readonly PixelFormat fmt = ref header.PixelFormat;

                    return fmt.RgbBitCount == rgbBitCount && fmt.RBitMask == rBitMask && fmt.GBitMask == gBitMask &&
                           fmt.BBitMask == bBitMask && fmt.ABitMask == aBitMask;
                }

                if (IsPixelFormat(32, 0xFF, 0xFF00, 0xFF0000, 0xFF000000))
                    Format = Graphics.PixelFormat.RGBA8;
                else if (IsPixelFormat(32, 0xFF0000, 0xFF00, 0xFF, 0xFF000000))
                    Format = Graphics.PixelFormat.BGRA8;
                else
                    throw new NotImplementedException();
                break;
            }
            
            default:
                throw new NotImplementedException();
        }

        ArraySize = 1;
        MipLevels = header.MipmapCount;
        
        if (MipLevels < 1)
            throw new NotSupportedException("DDS must have at least 1 mipmap level.");

        Bitmaps = new Bitmap[ArraySize, MipLevels];
        
        Size<int> size = new Size<int>((int) header.Width, (int) header.Height);
        Size = size;

        for (int i = 0; i < MipLevels; i++)
        {
            uint totalSize = CalculateSize(Format, (uint) size.Width, (uint) size.Height);
            byte[] bytes = reader.ReadBytes((int) totalSize);
            Bitmaps[0, i] = new Bitmap(size, bytes, Format);

            size = new Size<int>(size.Width >> 1, size.Height >> 1);
        }
    }

    private static uint CalculateSize(Graphics.PixelFormat format, uint width, uint height)
    {
        uint bppOrBlockSize;
        bool isCompressed = false;
        
        switch (format)
        {
            case Graphics.PixelFormat.RGBA8:
            case Graphics.PixelFormat.BGRA8:
                bppOrBlockSize = 32;
                break;

            case Graphics.PixelFormat.BC1:
            case Graphics.PixelFormat.BC1Srgb:
            case Graphics.PixelFormat.BC4U:
                bppOrBlockSize = 8;
                isCompressed = true;
                break;
            
            case Graphics.PixelFormat.BC2:
            case Graphics.PixelFormat.BC2Srgb:
            case Graphics.PixelFormat.BC3:
            case Graphics.PixelFormat.BC3Srgb:
            case Graphics.PixelFormat.BC5U:
            case Graphics.PixelFormat.BC6U:
            case Graphics.PixelFormat.BC6S:
            case Graphics.PixelFormat.BC7:
            case Graphics.PixelFormat.BC7Srgb:
                bppOrBlockSize = 16;
                isCompressed = true;
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }

        if (isCompressed)
            return uint.Max(1, (width + 3) >> 2) * bppOrBlockSize * (height >> 2); // TODO: Why does height need to be divided by 4?

        return ((width * bppOrBlockSize + 7) >> 3) * height;
    }

    private unsafe struct Header
    {
        public uint Size;
        public Flags Flags;
        public uint Height;
        public uint Width;
        public uint PitchOrLinearSize;
        public uint Depth;
        public uint MipmapCount;
        public fixed uint Reserved[11];
        public PixelFormat PixelFormat;
        public Caps Caps;
        public Caps2 Caps2;
        public uint Caps3;
        public uint Caps4;
        public uint Reserved2;
    }

    private struct HeaderDX10
    {
        public DxgiFormat Format;
        public ResourceDimension ResourceDimension;
        public MiscFlags MiscFlags;
        public uint ArraySize;
        public AlphaMode MiscFlags2;
    }

    private struct PixelFormat
    {
        public uint Size;
        public PixelFormatFlags Flags;
        public FourCC FourCC;
        public uint RgbBitCount;
        public uint RBitMask;
        public uint GBitMask;
        public uint BBitMask;
        public uint ABitMask;
    }

    [Flags]
    private enum Flags : uint
    {
        None = 0,
        
        Caps = 0x1,
        
        Height = 0x2,
        
        Width = 0x4,
        
        Pitch = 0x8,
        
        PixelFormat = 0x1000,
        
        MipmapCount = 0x20000,
        
        LinearSize = 0x80000,
        
        Depth = 0x800000
    }

    [Flags]
    private enum PixelFormatFlags : uint
    {
        None = 0,
        
        AlphaPixels = 0x1,
        
        Alpha = 0x2,
        
        FourCC = 0x4,
        
        RGB = 0x40,
        
        YUV = 0x200,
        
        Luminance = 0x20000
    }

    [Flags]
    private enum Caps : uint
    {
        None = 0,
        
        Complex = 0x8,
        
        Mipmap = 0x400000,
        
        Texture = 0x1000
    }

    [Flags]
    private enum Caps2 : uint
    {
        None = 0,
        
        Cubemap = 0x200,
        
        CubemapPositiveX = 0x400,
        
        CubemapNegativeX = 0x800,
        
        CubemapPositiveY = 0x1000,
        
        CubemapNegativeY = 0x2000,
        
        CubemapPositiveZ = 0x4000,
        
        CubemapNegativeZ = 0x8000,
        
        Volume = 0x200000
    }

    [Flags]
    private enum MiscFlags
    {
        None = 0,
        
        TextureCube = 0x4
    }

    private enum AlphaMode
    {
        Unknown = 0x0,
        
        Straight = 0x1,
        
        Premultiplied = 0x2,
        
        Opaque = 0x3,
        
        Custom = 0x4
    }

    private enum FourCC : uint
    {
        None = 0,
        
        DXT1 = 0x31545844,
        
        DXT2 = 0x32545844,
        
        DXT3 = 0x33545844,
        
        DXT4 = 0x34545844,
        
        DXT5 = 0x35545844,
        
        DX10 = 0x30315844,
        
        BC4U = 0x55344342,
        
        BC5U = 0x55354342
    }

    private enum ResourceDimension : uint
    {
        Unknown = 0,
        
        Buffer = 1,
        
        Texture1D = 2,
        
        Texture2D = 3,
        
        Texture3D = 4
    }

    private enum DxgiFormat : uint
    {
        Unknown = 0,
        R32G32B32A32Typeless = 1,
        R32G32B32A32Float = 2,
        R32G32B32A32Uint = 3,
        R32G32B32A32Sint = 4,
        R32G32B32Typeless = 5,
        R32G32B32Float = 6,
        R32G32B32Uint = 7,
        R32G32B32Sint = 8,
        R16G16B16A16Typeless = 9,
        R16G16B16A16Float = 10,
        R16G16B16A16Unorm = 11,
        R16G16B16A16Uint = 12,
        R16G16B16A16Snorm = 13,
        R16G16B16A16Sint = 14,
        R32G32Typeless = 15,
        R32G32Float = 16,
        R32G32Uint = 17,
        R32G32Sint = 18,
        R32G8X24Typeless = 19,
        D32FloatS8X24Uint = 20,
        R32FloatX8X24Typeless = 21,
        X32TypelessG8X24Uint = 22,
        R10G10B10A2Typeless = 23,
        R10G10B10A2Unorm = 24,
        R10G10B10A2Uint = 25,
        R11G11B10Float = 26,
        R8G8B8A8Typeless = 27,
        R8G8B8A8Unorm = 28,
        R8G8B8A8UnormSrgb = 29,
        R8G8B8A8Uint = 30,
        R8G8B8A8Snorm = 31,
        R8G8B8A8Sint = 32,
        R16G16Typeless = 33,
        R16G16Float = 34,
        R16G16Unorm = 35,
        R16G16Uint = 36,
        R16G16Snorm = 37,
        R16G16Sint = 38,
        R32Typeless = 39,
        D32Float = 40,
        R32Float = 41,
        R32Uint = 42,
        R32Sint = 43,
        R24G8Typeless = 44,
        D24UnormS8Uint = 45,
        R24UnormX8Typeless = 46,
        X24TypelessG8Uint = 47,
        R8G8Typeless = 48,
        R8G8Unorm = 49,
        R8G8Uint = 50,
        R8G8Snorm = 51,
        R8G8Sint = 52,
        R16Typeless = 53,
        R16Float = 54,
        D16Unorm = 55,
        R16Unorm = 56,
        R16Uint = 57,
        R16Snorm = 58,
        R16Sint = 59,
        R8Typeless = 60,
        R8Unorm = 61,
        R8Uint = 62,
        R8Snorm = 63,
        R8Sint = 64,
        A8Unorm = 65,
        R1Unorm = 66,
        R9G9B9E5Sharedexp = 67,
        R8G8B8G8Unorm = 68,
        G8R8G8B8Unorm = 69,
        Bc1Typeless = 70,
        Bc1Unorm = 71,
        Bc1UnormSrgb = 72,
        Bc2Typeless = 73,
        Bc2Unorm = 74,
        Bc2UnormSrgb = 75,
        Bc3Typeless = 76,
        Bc3Unorm = 77,
        Bc3UnormSrgb = 78,
        Bc4Typeless = 79,
        Bc4Unorm = 80,
        Bc4Snorm = 81,
        Bc5Typeless = 82,
        Bc5Unorm = 83,
        Bc5Snorm = 84,
        B5G6R5Unorm = 85,
        B5G5R5A1Unorm = 86,
        B8G8R8A8Unorm = 87,
        B8G8R8X8Unorm = 88,
        R10G10B10XrBiasA2Unorm = 89,
        B8G8R8A8Typeless = 90,
        B8G8R8A8UnormSrgb = 91,
        B8G8R8X8Typeless = 92,
        B8G8R8X8UnormSrgb = 93,
        Bc6HTypeless = 94,
        Bc6HUf16 = 95,
        Bc6HSf16 = 96,
        Bc7Typeless = 97,
        Bc7Unorm = 98,
        Bc7UnormSrgb = 99,
        Ayuv = 100,
        Y410 = 101,
        Y416 = 102,
        Nv12 = 103,
        P010 = 104,
        P016 = 105,
        _420_OPAQUE = 106,
        Yuy2 = 107,
        Y210 = 108,
        Y216 = 109,
        Nv11 = 110,
        Ai44 = 111,
        Ia44 = 112,
        P8 = 113,
        A8P8 = 114,
        B4G4R4A4Unorm = 115,
        P208 = 130,
        V208 = 131,
        V408 = 132,
        SamplerFeedbackMinMipOpaque = 189,
        SamplerFeedbackMipRegionUsedOpaque = 190,
        ForceUint = 0xffffffff
    }
}