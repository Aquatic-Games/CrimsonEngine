using System.Runtime.CompilerServices;
using SDL3;

namespace Crimson.Graphics.Utils;

internal static class SdlUtils
{
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