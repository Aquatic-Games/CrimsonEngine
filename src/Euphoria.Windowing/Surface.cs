using Euphoria.Core;
using Euphoria.Math;
using grabs.Graphics;
using SDL;
using static SDL.SDL3;

namespace Euphoria.Windowing;

/// <summary>
/// The primary application surface that is being rendered to.
/// </summary>
public static unsafe class Surface
{
    private static SDL_Window* _window;

    /// <summary>
    /// The surface's size, in pixels.
    /// </summary>
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
    
    /// <summary>
    /// The underlying handle(s) to the surface, typically provided by the window manager.
    /// </summary>
    /// <exception cref="PlatformNotSupportedException">Thrown if the current platform does not support the surface.</exception>
    public static SurfaceInfo Info
    {
        get
        {
            SurfaceInfo info;
            SDL_PropertiesID props = SDL_GetWindowProperties(_window);
            
            if (OperatingSystem.IsWindows())
            {
                nint hinstance = SDL_GetPointerProperty(props, SDL_PROP_WINDOW_WIN32_INSTANCE_POINTER, 0);
                nint hwnd = SDL_GetPointerProperty(props, SDL_PROP_WINDOW_WIN32_HWND_POINTER, 0);

                info = SurfaceInfo.Windows(hinstance, hwnd);
            }
            else if (OperatingSystem.IsLinux())
            {
                string driver = SDL_GetCurrentVideoDriver()!;

                if (driver == "wayland")
                {
                    nint display = SDL_GetPointerProperty(props, SDL_PROP_WINDOW_WAYLAND_DISPLAY_POINTER, 0);
                    nint surface = SDL_GetPointerProperty(props, SDL_PROP_WINDOW_WAYLAND_SURFACE_POINTER, 0);

                    info = SurfaceInfo.Wayland(display, surface);
                }
                else if (driver == "x11")
                {
                    nint display = SDL_GetPointerProperty(props, SDL_PROP_WINDOW_X11_DISPLAY_POINTER, 0);
                    nint window = (nint) SDL_GetNumberProperty(props, SDL_PROP_WINDOW_X11_WINDOW_NUMBER, 0);

                    info = SurfaceInfo.Xlib(display, window);
                }
                else
                    throw new PlatformNotSupportedException();
            }
            else
                throw new PlatformNotSupportedException();

            return info;
        }
    }

    /// <summary>
    /// Create the <see cref="Surface"/> with the given options.
    /// </summary>
    /// <param name="options">The <see cref="WindowOptions"/> to use when creating this surface.</param>
    /// <exception cref="Exception">Thrown if the surface failed to create.</exception>
    public static void Create(in WindowOptions options)
    {
        Logger.Trace("Initializing SDL.");
        if (!SDL_Init(SDL_InitFlags.SDL_INIT_VIDEO))
            throw new Exception($"Failed to initialize SDL: {SDL_GetError()}");

        Logger.Trace("Creating window.");
        _window = SDL_CreateWindow(options.Title, options.Size.Width, options.Size.Height, 0);

        if (_window == null)
            throw new Exception($"Failed to create window: {SDL_GetError()}");
    }

    /// <summary>
    /// Destroy the surface.
    /// </summary>
    public static void Destroy()
    {
        SDL_DestroyWindow(_window);
        SDL_Quit();
    }
}