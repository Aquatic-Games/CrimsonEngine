using System.Diagnostics;
using System.Runtime.CompilerServices;
using Silk.NET.SDL;

namespace Crimson.Graphics.Utils;

internal static class SdlUtils
{
    public static GPUColorTargetBlendState NonPremultipliedBlend => new()
    {
        EnableBlend = 1,
        SrcColorBlendfactor = GPUBlendFactor.SrcAlpha,
        DstColorBlendfactor = GPUBlendFactor.OneMinusSrcAlpha,
        DstAlphaBlendfactor = GPUBlendFactor.One,
        SrcAlphaBlendfactor = GPUBlendFactor.One,
        ColorBlendOp = GPUBlendOp.Add,
        AlphaBlendOp = GPUBlendOp.Add,
    };

    public static SDL.GPUColorTargetBlendState NoBlend => new()
    {
        EnableBlend = 0
    };
    
    public static IntPtr Check(this IntPtr ptr, string operation)
    {
        if (ptr == IntPtr.Zero)
            throw new Exception($"SDL operation '{operation}' failed: {SDL.GetError()}");

        return ptr;
    }

    public static void Check(this bool b, string operation)
    {
        if (!b)
            throw new Exception($"SDL operation '{operation}' failed: {SDL.GetError()}");
    }

    [Conditional("DEBUG")]
    public static void PushDebugGroup(IntPtr cb, string name)
    {
        // Doesn't work on directx?
        if (!OperatingSystem.IsWindows())
            SDL.PushGPUDebugGroup(cb, name);
    }

    [Conditional("DEBUG")]
    public static void PopDebugGroup(IntPtr cb)
    {
        if (!OperatingSystem.IsWindows())
            SDL.PopGPUDebugGroup(cb);
    }

    public static uint CalculateMipLevels(uint width, uint height)
    {
        return (uint) double.Floor(double.Log2(uint.Max(width, height))) + 1;
    }

    public static IntPtr CreateBuffer(IntPtr device, SDL.GPUBufferUsageFlags usage, uint size)
    {
        SDL.GPUBufferCreateInfo bufferInfo = new()
        {
            Usage = usage,
            Size = size
        };

        return SDL.CreateGPUBuffer(device, in bufferInfo).Check("Create buffer");
    }

    public static unsafe IntPtr CreateBuffer<T>(IntPtr device, SDL.GPUBufferUsageFlags usage, ReadOnlySpan<T> data) where T : unmanaged
    {
        uint size = (uint) (data.Length * sizeof(T));
        
        IntPtr buffer = CreateBuffer(device, usage, size);

        IntPtr cb = SDL.AcquireGPUCommandBuffer(device).Check("Acquire command buffer");
        Renderer.UpdateBuffer(cb, buffer, 0, data);
        SDL.SubmitGPUCommandBuffer(cb).Check("Submit command buffer");
        
        return buffer;
    }

    public static IntPtr CreateBuffer<T>(IntPtr device, SDL.GPUBufferUsageFlags usage, T[] data) where T : unmanaged
        => CreateBuffer<T>(device, usage, data.AsSpan());

    public static IntPtr CreateTransferBuffer(IntPtr device, SDL.GPUTransferBufferUsage usage, uint size)
    {
        SDL.GPUTransferBufferCreateInfo bufferInfo = new()
        {
            Usage = usage,
            Size = size
        };

        return SDL.CreateGPUTransferBuffer(device, in bufferInfo).Check("Create transfer buffer");
    }

    public static IntPtr CreateTexture2D(IntPtr device, uint width, uint height, SDL.GPUTextureFormat format,
        uint mipLevels, SDL.GPUTextureUsageFlags usage = SDL.GPUTextureUsageFlags.Sampler)
    {
        SDL.GPUTextureCreateInfo textureInfo = new()
        {
            Type = SDL.GPUTextureType.Texturetype2D,
            Width = width,
            Height = height,
            LayerCountOrDepth = 1,
            Format = format,
            Usage = usage,
            NumLevels = mipLevels == 0 ? CalculateMipLevels(width, height) : mipLevels,
            SampleCount = SDL.GPUSampleCount.SampleCount1
        };

        return SDL.CreateGPUTexture(device, in textureInfo).Check("Create texture");
    }

    public static unsafe IntPtr CreateTexture2D(IntPtr device, nint data, uint width, uint height,
        SDL.GPUTextureFormat format, uint mipLevels, SDL.GPUTextureUsageFlags usage = SDL.GPUTextureUsageFlags.Sampler)
    {
        IntPtr texture = CreateTexture2D(device, width, height, format, mipLevels, usage);

        uint size = SDL.CalculateGPUTextureFormatSize(format, width, height, 1);
        IntPtr transferBuffer = CreateTransferBuffer(device, SDL.GPUTransferBufferUsage.Upload, size);

        void* transferData = (void*) SDL.MapGPUTransferBuffer(device, transferBuffer, false);
        Unsafe.CopyBlock(transferData, (void*) data, size);
        SDL.UnmapGPUTransferBuffer(device, transferBuffer);

        IntPtr cb = SDL.AcquireGPUCommandBuffer(device).Check("Acquire command buffer");
        IntPtr pass = SDL.BeginGPUCopyPass(cb).Check("Begin copy pass");

        SDL.GPUTextureTransferInfo source = new()
        {
            TransferBuffer = transferBuffer,
            PixelsPerRow = width,
            RowsPerLayer = height,
            Offset = 0
        };

        SDL.GPUTextureRegion dest = new()
        {
            Texture = texture,
            X = 0,
            Y = 0,
            W = width,
            H = height,
            D = 1
        };
        
        SDL.UploadToGPUTexture(pass, in source, in dest, false);
        
        SDL.EndGPUCopyPass(pass);
        SDL.SubmitGPUCommandBuffer(cb).Check("Submit command buffer");
        
        SDL.ReleaseGPUTransferBuffer(device, transferBuffer);
        
        return texture;
    }

    public static SDL.GPUTextureFormat ToSdl(this PixelFormat format, out uint rowPitch)
    {
        switch (format)
        {
            case PixelFormat.RGBA8:
                rowPitch = 4;
                return SDL.GPUTextureFormat.R8G8B8A8Unorm;
            case PixelFormat.BGRA8:
                rowPitch = 4;
                return SDL.GPUTextureFormat.B8G8R8A8Unorm;
            case PixelFormat.BC1:
                rowPitch = 1;
                return SDL.GPUTextureFormat.BC1RGBAUnorm;
            case PixelFormat.BC1Srgb:
                rowPitch = 1;
                return SDL.GPUTextureFormat.BC1RGBAUnormSRGB;
            case PixelFormat.BC2:
                rowPitch = 2;
                return SDL.GPUTextureFormat.BC2RGBAUnorm;
            case PixelFormat.BC2Srgb:
                rowPitch = 2;
                return SDL.GPUTextureFormat.BC2RGBAUnorm;
            case PixelFormat.BC3:
                rowPitch = 2;
                return SDL.GPUTextureFormat.BC3RGBAUnorm;
            case PixelFormat.BC3Srgb:
                rowPitch = 2;
                return SDL.GPUTextureFormat.BC3RGBAUnormSRGB;
            case PixelFormat.BC4U:
                rowPitch = 1;
                return SDL.GPUTextureFormat.BC4RUnorm;
            /*case PixelFormat.BC4S:
                rowPitch = 1;
                return SDL.GPUTextureFormat.BC4RSnorm;*/
            case PixelFormat.BC5U:
                rowPitch = 2;
                return SDL.GPUTextureFormat.BC5RGUnorm;
            /*case PixelFormat.BC5S:
                break;*/
            case PixelFormat.BC6U:
                rowPitch = 2;
                return SDL.GPUTextureFormat.BC6HRGBUFloat;
            case PixelFormat.BC6S:
                rowPitch = 2;
                return SDL.GPUTextureFormat.BC6HRGBFloat;
            case PixelFormat.BC7:
                rowPitch = 2;
                return SDL.GPUTextureFormat.BC7RGBAUnorm;
            case PixelFormat.BC7Srgb:
                rowPitch = 2;
                return SDL.GPUTextureFormat.BC7RGBAUnormSRGB;
            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }
}