using Crimson.Math;
using Crimson.Platform;

namespace Crimson.Input.Bindings;

public struct MouseButtonBinding : IInputBinding
{
    private bool _isActive;
    private bool _becameActive;
    
    public MouseButton Button;
    
    public bool Enabled { get; set; }

    public InputSource Source => InputSource.Relative;

    public bool IsActive => _isActive;

    public bool BecameActive => _becameActive;

    public float Value => _isActive ? 1 : 0;

    public Vector2T<float> Value2D => _isActive ? Vector2T<float>.One : Vector2T<float>.Zero;

    public MouseButtonBinding(MouseButton button)
    {
        Button = button;
    }

    public void Update()
    {
        if (Enabled)
        {
            _isActive = Input.IsMouseButtonDown(Button);
            _becameActive = Input.IsMouseButtonPressed(Button);
        }
        else
        {
            _isActive = false;
            _becameActive = false;
        }
    }
}