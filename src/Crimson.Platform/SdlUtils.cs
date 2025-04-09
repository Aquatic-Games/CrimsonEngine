using Silk.NET.SDL;

namespace Crimson.Platform;

internal class SdlUtils
{
    public static Sdl SDL;

    static SdlUtils()
    {
        SDL = Sdl.GetApi();
    }
}