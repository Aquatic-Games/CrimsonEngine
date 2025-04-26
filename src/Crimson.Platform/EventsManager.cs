using System.Numerics;
using Crimson.Math;
using SDL3;

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

    public event OnSurfaceSizeChanged SurfaceSizeChanged = delegate { };

    public event OnKeyDown KeyDown = delegate { };

    public event OnKeyRepeat KeyRepeat = delegate { };

    public event OnKeyUp KeyUp = delegate { };

    public event OnMouseButtonDown MouseButtonDown = delegate { };

    public event OnMouseButtonUp MouseButtonUp = delegate { };

    public event OnMouseMove MouseMove = delegate { };

    public event OnMouseScroll MouseScroll = delegate { };

    public event OnTextInput TextInput = delegate { };

    /// <summary>
    /// Create a new <see cref="EventsManager"/>.
    /// </summary>
    public EventsManager()
    {
        if (!SDL.Init(SDL.InitFlags.Events))
            throw new Exception($"Failed to initialize SDL: {SDL.GetError()}");
    }

    /// <summary>
    /// Poll and process events.
    /// </summary>
    public unsafe void ProcessEvents()
    {
        SDL.Event sdlEvent;
        while (SDL.PollEvent(out sdlEvent))
        {
            switch ((SDL.EventType) sdlEvent.Type)
            {
                case SDL.EventType.WindowCloseRequested:
                {
                    WindowClose();
                    break;
                }
                case SDL.EventType.WindowResized:
                {
                    SurfaceSizeChanged(new Size<int>(sdlEvent.Window.Data1, sdlEvent.Window.Data2));
                    break;
                }
                
                case SDL.EventType.KeyDown:
                {
                    if (sdlEvent.Key.Repeat)
                        break;
                    KeyDown(SdlUtils.KeycodeToKey(sdlEvent.Key.Key));
                    break;
                }
                case SDL.EventType.KeyUp:
                {
                    KeyUp(SdlUtils.KeycodeToKey(sdlEvent.Key.Key));
                    break;
                }

                case SDL.EventType.MouseButtonDown:
                {
                    MouseButtonDown(SdlUtils.ButtonIndexToButton(sdlEvent.Button.Button));
                    break;
                }
                case SDL.EventType.MouseButtonUp:
                {
                    MouseButtonUp(SdlUtils.ButtonIndexToButton(sdlEvent.Button.Button));
                    break;
                }
                case SDL.EventType.MouseMotion:
                {
                    MouseMove(new Vector2(sdlEvent.Motion.X, sdlEvent.Motion.Y),
                        new Vector2(sdlEvent.Motion.XRel, sdlEvent.Motion.YRel));
                    break;
                }
                case SDL.EventType.MouseWheel:
                {
                    MouseScroll(new Vector2(sdlEvent.Wheel.X, sdlEvent.Wheel.Y));
                    break;
                }

                case SDL.EventType.TextInput :
                {
                    string text = new string((sbyte*) sdlEvent.Text.Text);
                    Console.WriteLine(text);
                    foreach (char c in text)
                        TextInput(c);
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

    public delegate void OnSurfaceSizeChanged(Size<int> newSize);
    
    public delegate void OnKeyDown(Key key);

    public delegate void OnKeyRepeat(Key key);

    public delegate void OnKeyUp(Key key);

    public delegate void OnMouseButtonDown(MouseButton button);

    public delegate void OnMouseButtonUp(MouseButton button);
    
    public delegate void OnMouseMove(Vector2 position, Vector2 delta);

    public delegate void OnMouseScroll(Vector2 scroll);

    public delegate void OnTextInput(char c);
}