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
}