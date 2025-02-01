using SDL;
using static SDL.SDL_EventType;
using static SDL.SDL3;

namespace Euphoria.Engine;

public sealed unsafe class Window : IDisposable
{
    public event OnClose Close = delegate { };

    private readonly SDL_Window* _window;
    
    public Window(in WindowInfo info)
    {
        if (!SDL_Init(SDL_InitFlags.SDL_INIT_VIDEO | SDL_InitFlags.SDL_INIT_EVENTS))
            throw new Exception($"Failed to initialize SDL: {SDL_GetError()}");

        _window = SDL_CreateWindow(info.Title, info.Size.Width, info.Size.Height, SDL_WindowFlags.SDL_WINDOW_VULKAN);

        if (_window == null)
            throw new Exception($"Failed to create window: {SDL_GetError()}");
    }

    internal void ProcessEvents()
    {
        SDL_Event winEvent;
        while (SDL_PollEvent(&winEvent))
        {
            switch (winEvent.Type)
            {
                case SDL_EVENT_WINDOW_CLOSE_REQUESTED:
                    Close();
                    break;
            }
        }
    }

    public void Dispose()
    {
        SDL_DestroyWindow(_window);
        SDL_Quit();
    }

    public delegate void OnClose();
}