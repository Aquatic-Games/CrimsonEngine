using System.Numerics;
using Crimson.Math;
using Crimson.Platform;

namespace Crimson.Input.Bindings;

public struct KeyBinding : IInputBinding
{
    private bool _isActive;
    private bool _becameActive;
    
    public Key Key;

    public bool Enabled { get; set; }

    public InputSource Source => InputSource.Relative;

    public bool IsActive => _isActive;

    public bool BecameActive => _becameActive;

    public float Value => _isActive ? 1 : 0;

    public Vector2T<float> Value2D => _isActive ? Vector2T<float>.One : Vector2T<float>.Zero;

    public KeyBinding(Key key)
    {
        Key = key;
    }
    
    public void Update()
    {
        if (Enabled)
        {
            _isActive = Input.IsKeyDown(Key);
            _becameActive = Input.IsKeyPressed(Key);
        }
        else
        {
            _isActive = false;
            _becameActive = false;
        }
    }
}