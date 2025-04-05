using Silk.NET.SDL;
using static Euphoria.Windowing.SdlUtils;

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
        Event sdlEvent;
        while (SDL.PollEvent(&sdlEvent) != 0)
        {
            switch ((EventType) sdlEvent.Type)
            {
                case EventType.Windowevent:
                {
                    switch ((WindowEventID) sdlEvent.Window.Event)
                    {
                        case WindowEventID.Close:
                            WindowClose();
                            break;
                    }
                    
                    break;
                }
            }
        }
    }
    
    /// <summary>
    /// A delegate that is used in the <see cref="Events.WindowClose"/> event.
    /// </summary>
    public delegate void OnWindowClose();
}