using System.Diagnostics;
using System.Runtime.CompilerServices;
using SDL3;

namespace Crimson.Graphics.Utils;

internal static class SdlUtils
{
    public static SDL.GPUColorTargetBlendState NonPremultipliedBlend => new()
    {
        EnableBlend = 1,
        SrcColorBlendfactor = SDL.GPUBlendFactor.SrcAlpha,
        DstColorBlendfactor = SDL.GPUBlendFactor.OneMinusSrcAlpha,
        DstAlphaBlendfactor = SDL.GPUBlendFactor.One,
        SrcAlphaBlendfactor = SDL.GPUBlendFactor.One,
        ColorBlendOp = SDL.GPUBlendOp.Add,
        AlphaBlendOp = SDL.GPUBlendOp.Add,
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

    public static unsafe IntPtr CreateBuffer<T>(IntPtr device, SDL.GPUBufferUsageFlags usage, T[] data) where T : unmanaged
    {
        uint size = (uint) (data.Length * sizeof(T));
        
        IntPtr buffer = CreateBuffer(device, usage, size);
        IntPtr transferBuffer = CreateTransferBuffer(device, SDL.GPUTransferBufferUsage.Upload, size);

        void* mapData = (void*) SDL.MapGPUTransferBuffer(device, transferBuffer, false).Check("Map transfer buffer");
        fixed (void* pData = data)
            Unsafe.CopyBlock(mapData, pData, size);
        SDL.UnmapGPUTransferBuffer(device, transferBuffer);

        IntPtr cb = SDL.AcquireGPUCommandBuffer(device).Check("Acquire command buffer");
        IntPtr pass = SDL.BeginGPUCopyPass(cb).Check("Begin copy pass");

        SDL.GPUTransferBufferLocation source = new()
        {
            TransferBuffer = transferBuffer,
            Offset = 0
        };

        SDL.GPUBufferRegion dest = new()
        {
            Buffer = buffer,
            Offset = 0,
            Size = size
        };

        SDL.UploadToGPUBuffer(pass, in source, in dest, false);
        
        SDL.EndGPUCopyPass(pass);
        SDL.SubmitGPUCommandBuffer(cb).Check("Submit command buffer");
        
        SDL.ReleaseGPUTransferBuffer(device, transferBuffer);
        
        return buffer;
    }

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
            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }
}