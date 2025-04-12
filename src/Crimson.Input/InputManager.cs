using Crimson.Platform;

namespace Crimson.Input;

public class InputManager
{
    private readonly HashSet<Key> _keysDown;
    private readonly HashSet<Key> _keysPressed;
    
    public InputManager(EventsManager events)
    {
        _keysDown = [];
        _keysPressed = [];
        
        events.KeyDown += OnKeyDown;
        events.KeyUp += OnKeyUp;
    }

    public bool IsKeyDown(Key key)
        => _keysDown.Contains(key);

    public bool IsKeyPressed(Key key)
        => _keysPressed.Contains(key);

    public void Update()
    {
        _keysPressed.Clear();
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
}