using Silk.NET.SDL;

namespace Euphoria.Windowing;

internal class SdlUtils
{
    public static Sdl SDL;

    static SdlUtils()
    {
        SDL = Sdl.GetApi();
    }
}