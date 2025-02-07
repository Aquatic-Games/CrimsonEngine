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
}