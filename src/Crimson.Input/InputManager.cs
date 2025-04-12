using System.Numerics;
using Crimson.Platform;

namespace Crimson.Input;

public class InputManager
{
    private readonly HashSet<Key> _keysDown;
    private readonly HashSet<Key> _keysPressed;

    private Vector2 _mousePosition;
    private Vector2 _mouseDelta;

    public Vector2 MousePosition => _mousePosition;

    public Vector2 MouseDelta => _mouseDelta;
    
    public InputManager(EventsManager events)
    {
        _keysDown = [];
        _keysPressed = [];
        
        events.KeyDown += OnKeyDown;
        events.KeyUp += OnKeyUp;
        events.MouseMove += OnMouseMove;
    }

    public bool IsKeyDown(Key key)
        => _keysDown.Contains(key);

    public bool IsKeyPressed(Key key)
        => _keysPressed.Contains(key);

    public void Update()
    {
        _keysPressed.Clear();
        _mouseDelta = Vector2.Zero;
    }
    
    private void OnKeyDown(Key key)
    {
        _keysDown.Add(key);
        _keysPressed.Add(key);
    }

    private void OnKeyUp(Key key)
    {
        _keysDown.Remove(key);
        _keysPressed.Remove(key);
    }
    
    private void OnMouseMove(Vector2 position, Vector2 delta)
    {
        _mousePosition = position;
        _mouseDelta += delta;
    }
}