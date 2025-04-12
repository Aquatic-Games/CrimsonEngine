using System.Numerics;
using Silk.NET.SDL;
using static Crimson.Platform.SdlUtils;

namespace Crimson.Platform;

/// <summary>
/// Contains a series of events that may be called in an application.
/// </summary>
public class EventsManager : IDisposable
{
    /// <summary>
    /// Invoked when the window is requesting to close.
    /// </summary>
    public event OnWindowClose WindowClose = delegate { };

    public event OnKeyDown KeyDown = delegate { };

    public event OnKeyRepeat KeyRepeat = delegate { };

    public event OnKeyUp KeyUp = delegate { };

    public event OnMouseMove MouseMove = delegate { };

    /// <summary>
    /// Create a new <see cref="EventsManager"/>.
    /// </summary>
    public EventsManager()
    {
        if (SDL.Init(Sdl.InitEvents) < 0)
            throw new Exception($"Failed to initialize SDL: {SDL.GetErrorS()}");
    }

    /// <summary>
    /// Poll and process events.
    /// </summary>
    public unsafe void ProcessEvents()
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
                
                case EventType.Keydown:
                {
                    KeyDown(KeycodeToKey((KeyCode) sdlEvent.Key.Keysym.Sym));
                    break;
                }
                case EventType.Keyup:
                {
                    KeyUp(KeycodeToKey((KeyCode) sdlEvent.Key.Keysym.Sym));
                    break;
                }

                case EventType.Mousemotion:
                {
                    MouseMove(new Vector2(sdlEvent.Motion.X, sdlEvent.Motion.Y),
                        new Vector2(sdlEvent.Motion.Xrel, sdlEvent.Motion.Yrel));
                    break;
                }
            }
        }
    }
    
    /// <summary>
    /// Dispose of this <see cref="EventsManager"/>.
    /// </summary>
    public void Dispose()
    {
        SDL.Quit();
    }
    
    /// <summary>
    /// A delegate that is used in the <see cref="Events.WindowClose"/> event.
    /// </summary>
    public delegate void OnWindowClose();

    
    public delegate void OnKeyDown(Key key);

    public delegate void OnKeyRepeat(Key key);

    public delegate void OnKeyUp(Key key);

    public delegate void OnMouseMove(Vector2 position, Vector2 delta);
}