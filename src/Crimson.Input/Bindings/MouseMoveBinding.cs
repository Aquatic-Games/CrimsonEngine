using Crimson.Math;

namespace Crimson.Input.Bindings;

public class MouseMoveBinding : IInputBinding
{
    private Vector2T<float> _mousePosition;
    private float _value;
    private bool _isActive;
    private bool _becameActive;
    
    public bool Enabled { get; set; }

    public InputSource Source => InputSource.Absolute;

    public bool IsActive => _isActive;

    public bool BecameActive => _becameActive;

    public float Value => _value;

    public Vector2T<float> Value2D => _mousePosition;
    
    public void Update()
    {
        if (Enabled)
        {
            _mousePosition = Input.MouseDelta;
            _value = Vector2T.Magnitude(_mousePosition);
            bool isActive = _mousePosition != Vector2T<float>.Zero;
            _becameActive = isActive && !_isActive;
            _isActive = isActive;
        }
        else
        {
            _mousePosition = Vector2T<float>.Zero;
            _value = 0;
            _isActive = false;
            _becameActive = false;
        }
    }
}