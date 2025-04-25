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