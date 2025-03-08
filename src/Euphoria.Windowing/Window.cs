using Euphoria.Math;
using SDL;
using static SDL.SDL3;

namespace Euphoria.Windowing;

public static unsafe class Window
{
    private static SDL_Window* _window;

    public static Size<int> Size
    {
        get
        {
            int w, h;
            SDL_GetWindowSizeInPixels(_window, &w, &h);

            return new Size<int>(w, h);
        }
        set => SDL_SetWindowSize(_window, value.Width, value.Height);
    }

    public static void Create(in WindowOptions options)
    {
        if (!SDL_Init(SDL_InitFlags.SDL_INIT_VIDEO))
            throw new Exception($"Failed to initialize SDL: {SDL_GetError()}");

        _window = SDL_CreateWindow(options.Title, options.Size.Width, options.Size.Height, 0);

        if (_window == null)
            throw new Exception($"Failed to create window: {SDL_GetError()}");
    }

    public static void Destroy()
    {
        SDL_DestroyWindow(_window);
        SDL_Quit();
    }
}