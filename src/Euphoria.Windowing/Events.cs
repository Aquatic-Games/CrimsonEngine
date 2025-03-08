using SDL;
using static SDL.SDL3;

namespace Euphoria.Windowing;

public static class Events
{
    public static event OnWindowClose WindowClose = delegate { };

    public static unsafe void ProcessEvents()
    {
        SDL_Event sdlEvent;
        while (SDL_PollEvent(&sdlEvent))
        {
            switch (sdlEvent.Type)
            {
                case SDL_EventType.SDL_EVENT_WINDOW_CLOSE_REQUESTED:
                    WindowClose();
                    break;
            }
        }
    }
    
    public delegate void OnWindowClose();
}