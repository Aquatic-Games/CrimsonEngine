using SDL;
using static SDL.SDL3;

namespace Euphoria.Graphics;

internal static class SdlUtil
{
    public static void Check(bool result, string operation)
    {
        if (!result)
            throw new Exception($"SDL operation '{operation}' failed: {SDL_GetError()}");
    }

    public static unsafe T* Check<T>(T* result, string operation) where T : unmanaged
    {
        if (result == null)
            throw new Exception($"SDL operation '{operation}' failed: {SDL_GetError()}");

        return result;
    }

    public static unsafe void UploadTransferBuffer(SDL_GPUCopyPass* pass, SDL_GPUTransferBuffer* transBuffer,
        SDL_GPUBuffer* buffer, uint size, uint transferOffset, uint destOffset = 0)
    {
        SDL_GPUTransferBufferLocation srcRegion = new SDL_GPUTransferBufferLocation()
        {
            transfer_buffer = transBuffer,
            offset = transferOffset
        };

        SDL_GPUBufferRegion destRegion = new SDL_GPUBufferRegion()
        {
            buffer = buffer,
            offset = destOffset,
            size = size
        };
        
        SDL_UploadToGPUBuffer(pass, &srcRegion, &destRegion, true);
    }
}