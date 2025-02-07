using SDL;
using static SDL.SDL_EventType;
using static SDL.SDL3;

namespace Euphoria.Engine;

public sealed unsafe class Window : IDisposable
{
    public event OnClose Close = delegate { };

    internal readonly SDL_Window* Handle;
    
    public Window(in WindowInfo info)
    {
        if (!SDL_Init(SDL_InitFlags.SDL_INIT_VIDEO | SDL_InitFlags.SDL_INIT_EVENTS))
            throw new Exception($"Failed to initialize SDL: {SDL_GetError()}");

        Handle = SDL_CreateWindow(info.Title, info.Size.Width, info.Size.Height, 0);

        if (Handle == null)
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
        SDL_DestroyWindow(Handle);
        SDL_Quit();
    }

    public delegate void OnClose();
}