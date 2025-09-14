using Crimson.Input.Bindings;
using Crimson.Math;

namespace Crimson.Input;

public sealed class InputAction
{
    private bool _enabled;
    private InputSource _source;
    private bool _isDown;
    private bool _isPressed;
    private float _value;
    private Vector2T<float> _value2D;
    
    public readonly string Name;
    
    public readonly IInputBinding[] Bindings;

    public bool Enabled
    {
        get => _enabled;
        internal set
        {
            _enabled = value;
            foreach (IInputBinding binding in Bindings)
                binding.Enabled = value;
        }
    }

    public InputSource Source => _source;

    public bool IsDown => _isDown;
    
    public bool IsPressed => _isPressed;

    public float Value => _value;

    public Vector2T<float> Value2D => _value2D;
    
    public InputAction(string name, params IInputBinding[] bindings)
    {
        Name = name;
        Bindings = bindings;
    }

    public void Update()
    {
        foreach (IInputBinding binding in Bindings)
            binding.Update();

        _isDown = false;
        _isPressed = false;
        _value = 0;
        _value2D = Vector2T<float>.Zero;
        
        foreach (IInputBinding binding in Bindings)
        {
            if (binding.IsActive)
            {
                _isDown = true;
                _source = Bindings[0].Source;
            }

            if (binding.BecameActive)
                _isPressed = true;
            
            _value += binding.Value;
            _value2D += binding.Value2D;
        }
    }
}