using SDL;
using static SDL.SDL3;

namespace Euphoria.Windowing;

/// <summary>
/// Contains a series of events that may be called in an application.
/// </summary>
public static class Events
{
    /// <summary>
    /// Invoked when the window is requesting to close.
    /// </summary>
    public static event OnWindowClose WindowClose = delegate { };

    /// <summary>
    /// Poll and process events.
    /// </summary>
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
    
    /// <summary>
    /// A delegate that is used in the <see cref="Events.WindowClose"/> event.
    /// </summary>
    public delegate void OnWindowClose();
}