using System.Numerics;
using Crimson.Platform;

namespace Crimson.Input;

public static class Input
{
    private static readonly HashSet<Key> _keysDown;
    private static readonly HashSet<Key> _keysPressed;

    private static readonly HashSet<MouseButton> _buttonsDown;
    private static readonly HashSet<MouseButton> _buttonsPressed;

    private static Vector2 _mousePosition;
    private static Vector2 _mouseDelta;
    private static Vector2 _scrollDelta;

    public static Vector2 MousePosition => _mousePosition;

    public static Vector2 MouseDelta => _mouseDelta;

    public static Vector2 ScrollDelta => _scrollDelta;

    static Input()
    {
        _keysDown = [];
        _keysPressed = [];
        _buttonsDown = [];
        _buttonsPressed = [];
    }
    
    public static void Create()
    {
        _keysDown.Clear();
        _keysPressed.Clear();
        _buttonsDown.Clear();
        _buttonsPressed.Clear();
        
        Events.KeyDown += OnKeyDown;
        Events.KeyUp += OnKeyUp;
        Events.MouseButtonDown += OnMouseButtonDown;
        Events.MouseButtonUp += OnMouseButtonUp;
        Events.MouseMove += OnMouseMove;
        Events.MouseScroll += OnMouseScroll;
    }

    public static void Destroy()
    {
        Events.KeyDown -= OnKeyDown;
        Events.KeyUp -= OnKeyUp;
        Events.MouseButtonDown -= OnMouseButtonDown;
        Events.MouseButtonUp -= OnMouseButtonUp;
        Events.MouseMove -= OnMouseMove;
        Events.MouseScroll -= OnMouseScroll;
    }

    public static bool IsKeyDown(Key key)
        => _keysDown.Contains(key);

    public static bool IsKeyPressed(Key key)
        => _keysPressed.Contains(key);

    public static bool IsMouseButtonDown(MouseButton button)
        => _buttonsDown.Contains(button);

    public static bool IsMouseButtonPressed(MouseButton button)
        => _buttonsPressed.Contains(button);

    public static void Update()
    {
        _keysPressed.Clear();
        _buttonsPressed.Clear();
        _mouseDelta = Vector2.Zero;
        _scrollDelta = Vector2.Zero;
    }
    
    private static void OnKeyDown(Key key)
    {
        _keysDown.Add(key);
        _keysPressed.Add(key);
    }

    private static void OnKeyUp(Key key)
    {
        _keysDown.Remove(key);
        _keysPressed.Remove(key);
    }
    
    private static void OnMouseButtonDown(MouseButton button)
    {
        _buttonsDown.Add(button);
        _buttonsPressed.Add(button);
    }
    
    private static void OnMouseButtonUp(MouseButton button)
    {
        _buttonsDown.Remove(button);
        _buttonsPressed.Remove(button);
    }
    
    private static void OnMouseMove(Vector2 position, Vector2 delta)
    {
        _mousePosition = position;
        _mouseDelta += delta;
    }
    
    private static void OnMouseScroll(Vector2 scroll)
    {
        _scrollDelta += scroll;
    }
}