using Crimson.Input.Bindings;
using Crimson.Math;

namespace Crimson.Input;

public sealed class InputAction
{
    private bool _enabled;
    private InputSource _source;
    
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

    public bool IsDown
    {
        get
        {
            foreach (IInputBinding binding in Bindings)
            {
                if (!binding.Active)
                    continue;
                _source = binding.Source;
                return true;
            }

            return false;
        }
    }

    public float Value
    {
        get
        {
            float value = 0;

            foreach (IInputBinding binding in Bindings)
            {
                value += binding.Value;
                if (binding.Active)
                    _source = binding.Source;
            }
            
            return value;
        }
    }

    public Vector2T<float> Value2D
    {
        get
        {
            Vector2T<float> value = Vector2T<float>.Zero;
            
            foreach (IInputBinding binding in Bindings)
            {
                value += binding.Value2D;
                if (binding.Active)
                    _source = binding.Source;
            }

            return value;
        }
    }
    
    public InputAction(string name, params IInputBinding[] bindings)
    {
        Name = name;
        Bindings = bindings;
    }
}