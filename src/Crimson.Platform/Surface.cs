using Crimson.Core;
using Crimson.Graphics;
using Crimson.Math;
using SDL3;

namespace Crimson.Platform;

/// <summary>
/// The primary application surface that is being rendered to.
/// </summary>
public static class Surface
{
    private static IntPtr _window;

    /// <summary>
    /// The surface's size, in pixels.
    /// </summary>
    /// <remarks><see cref="set_Size"/> may not be available on all platforms.</remarks>
    public static Size<int> Size
    {
        get
        {
            SDL.GetWindowSizeInPixels(_window, out int w, out int h);

            return new Size<int>(w, h);
        }
        set => SDL.SetWindowSize(_window, value.Width, value.Height);
    }

    /// <summary>
    /// Gets/sets if the cursor is visible. If false, the cursor will be invisible and locked to the surface.
    /// </summary>
    /// <remarks>Only works on platforms that support mouse input.</remarks>
    public static bool CursorVisible
    {
        get => SDL.GetWindowRelativeMouseMode(_window);
        set => SDL.SetWindowRelativeMouseMode(_window, !value);
    }
    
    /// <summary>
    /// Allow/disallow text input. This may cause on-screen keyboards to appear, etc.
    /// </summary>
    public static bool AllowTextInput
    {
        get => SDL.TextInputActive(_window);
        set
        {
            if (value)
                SDL.StartTextInput(_window);
            else
                SDL.StopTextInput(_window);
        }
    }
    
    /// <summary>
    /// The underlying handle(s) to the surface, typically provided by the window manager.
    /// </summary>
    /// <exception cref="PlatformNotSupportedException">Thrown if the current platform does not support the surface.</exception>
    public static SurfaceInfo Info
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

            return new SurfaceInfo(_window, Size);
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
        if (!SDL.Init(SDL.InitFlags.Video))
            throw new Exception($"Failed to initialize SDL: {SDL.GetError()}");
        
        SDL.WindowFlags flags = SDL.WindowFlags.InputFocus | SDL.WindowFlags.MouseFocus | SDL.WindowFlags.Vulkan;

        if (options.Resizable)
            flags |= SDL.WindowFlags.Resizable;

        if (options.FullScreen)
            flags |= SDL.WindowFlags.Fullscreen;
        
        Logger.Trace("Creating window.");
        _window = SDL.CreateWindow(options.Title, options.Size.Width, options.Size.Height, flags);

        if (_window == IntPtr.Zero)
            throw new Exception($"Failed to create window: {SDL.GetError()}");
    }

    /// <summary>
    /// Destroy the surface.
    /// </summary>
    public static void Destroy()
    {
        SDL.DestroyWindow(_window);
    }
}