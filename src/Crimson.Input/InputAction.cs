using Crimson.Input.Bindings;
using Crimson.Math;

namespace Crimson.Input;

public sealed class InputAction
{
    private bool _enabled;
    
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
    
    public bool IsPressed => Bindings.Any(binding => binding.Pressed);

    public float Value => Bindings.Sum(binding => binding.Value);

    public Vector2T<float> Value2D
    {
        get
        {
            Vector2T<float> value = Vector2T<float>.Zero;
            foreach (IInputBinding binding in Bindings)
                value += binding.Value2D;
            return value;
        }
    }
    
    public InputAction(string name, params IInputBinding[] bindings)
    {
        Name = name;
        Bindings = bindings;
    }
}