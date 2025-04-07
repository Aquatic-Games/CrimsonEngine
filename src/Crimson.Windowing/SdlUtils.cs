using Silk.NET.SDL;

namespace Crimson.Windowing;

internal class SdlUtils
{
    public static Sdl SDL;

    static SdlUtils()
    {
        SDL = Sdl.GetApi();
    }
}