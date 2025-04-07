using Crimson.Core;
using Crimson.Math;
using Crimson.Render;
using Silk.NET.SDL;
using static Crimson.Windowing.SdlUtils;

namespace Crimson.Windowing;

/// <summary>
/// The primary application surface that is being rendered to.
/// </summary>
public sealed unsafe class Surface : IDisposable
{
    private Window* _window;

    /// <summary>
    /// The surface's size, in pixels.
    /// </summary>
    public Size<int> Size
    {
        get
        {
            int w, h;
            SDL.GetWindowSizeInPixels(_window, &w, &h);

            return new Size<int>(w, h);
        }
        set => SDL.SetWindowSize(_window, value.Width, value.Height);
    }
    
    /// <summary>
    /// The underlying handle(s) to the surface, typically provided by the window manager.
    /// </summary>
    /// <exception cref="PlatformNotSupportedException">Thrown if the current platform does not support the surface.</exception>
    public SurfaceInfo Info
    {
        get
        {
            /*SurfaceInfo info;
            SDL_PropertiesID props = SDL_GetWindowProperties(_window);
            
            if (OperatingSystem.IsWindows())
            {
                nint hinstance = SDL_GetPointerProperty(props, SDL_PROP_WINDOW_WIN32_INSTANCE_POINTER, 0);
                nint hwnd = SDL_GetPointerProperty(props, SDL_PROP_WINDOW_WIN32_HWND_POINTER, 0);

                //info = SurfaceInfo.Windows(hinstance, hwnd);
                info = new SurfaceInfo(hwnd);
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

            return info;*/

            SurfaceInfo info;

            if (OperatingSystem.IsWindows())
            {
                SysWMInfo wmInfo = new SysWMInfo();
                SDL.GetVersion(&wmInfo.Version);
                SDL.GetWindowWMInfo(_window, &wmInfo);

                info = new SurfaceInfo(wmInfo.Info.Win.Hwnd);
            }
            else
            {
                info = new SurfaceInfo((nint) _window);
            }

            return info;
        }
    }

    /// <summary>
    /// Create the <see cref="Surface"/> with the given options.
    /// </summary>
    /// <param name="options">The <see cref="WindowOptions"/> to use when creating this surface.</param>
    /// <exception cref="Exception">Thrown if the surface failed to create.</exception>
    public Surface(in WindowOptions options)
    {
        Logger.Trace("Initializing SDL.");
        if (SDL.Init(Sdl.InitVideo) < 0)
            throw new Exception($"Failed to initialize SDL: {SDL.GetErrorS()}");

        WindowFlags flags = 0;

        if (!OperatingSystem.IsWindows())
            flags |= WindowFlags.Vulkan;
        
        Logger.Trace("Creating window.");
        _window = SDL.CreateWindow(options.Title, Sdl.WindowposCentered, Sdl.WindowposCentered, options.Size.Width,
            options.Size.Height, (uint) flags);

        if (_window == null)
            throw new Exception($"Failed to create window: {SDL.GetErrorS()}");
    }

    /// <summary>
    /// Destroy the surface.
    /// </summary>
    public void Dispose()
    {
        SDL.DestroyWindow(_window);
        SDL.Quit();
    }
}