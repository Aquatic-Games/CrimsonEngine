using System.Numerics;
using Crimson.Math;
using Crimson.Platform;

namespace Crimson.Input;

public static class Input
{
    private static Dictionary<string, ActionSet> _actionSets;
    
    private static readonly HashSet<Key> _keysDown;
    private static readonly HashSet<Key> _keysPressed;

    private static readonly HashSet<MouseButton> _buttonsDown;
    private static readonly HashSet<MouseButton> _buttonsPressed;

    private static Vector2T<float> _mousePosition;
    private static Vector2T<float> _mouseDelta;
    private static Vector2T<float> _scrollDelta;

    public static Vector2T<float> MousePosition => _mousePosition;

    public static Vector2T<float> MouseDelta => _mouseDelta;

    public static Vector2T<float> ScrollDelta => _scrollDelta;

    static Input()
    {
        _actionSets = [];
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

    public static void AddActionSet(ActionSet set)
    {
        _actionSets.Add(set.Name, set);
    }

    public static ActionSet GetActionSet(string name)
    {
        return _actionSets[name];
    }

    public static void SetCurrentActionSet(string name)
    {
        foreach ((_, ActionSet set) in _actionSets)
            set.Enabled = false;

        ActionSet s = _actionSets[name];
        s.Enabled = true;
        Surface.CursorVisible = s.CursorVisible;
    }

    public static bool IsKeyDown(Key key)
        => _keysDown.Contains(key);

    public static bool IsKeyDown(out Key key, params ReadOnlySpan<Key> keys)
    {
        foreach (Key k in keys)
        {
            if (_keysDown.Contains(k))
            {
                key = k;
                return true;
            }
        }

        key = Key.Unknown;
        return false;
    }

    public static bool IsKeyPressed(Key key)
        => _keysPressed.Contains(key);
    
    public static bool IsKeyPressed(out Key key, params ReadOnlySpan<Key> keys)
    {
        foreach (Key k in keys)
        {
            if (_keysPressed.Contains(k))
            {
                key = k;
                return true;
            }
        }

        key = Key.Unknown;
        return false;
    }

    public static bool IsMouseButtonDown(MouseButton button)
        => _buttonsDown.Contains(button);

    public static bool IsMouseButtonPressed(MouseButton button)
        => _buttonsPressed.Contains(button);

    public static void Update()
    {
        _keysPressed.Clear();
        _buttonsPressed.Clear();
        _mouseDelta = Vector2T<float>.Zero;
        _scrollDelta = Vector2T<float>.Zero;
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
    
    private static void OnMouseMove(Vector2T<float> position, Vector2T<float> delta)
    {
        _mousePosition = position;
        _mouseDelta += delta;
    }
    
    private static void OnMouseScroll(Vector2T<float> scroll)
    {
        _scrollDelta += scroll;
    }
}