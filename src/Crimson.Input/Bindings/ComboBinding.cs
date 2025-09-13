using Crimson.Math;

namespace Crimson.Input.Bindings;

public struct ComboBinding : IInputBinding
{
    private bool _enabled;
    
    public IInputBinding[] Bindings;

    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            foreach (IInputBinding binding in Bindings)
                binding.Enabled = value;
        }
    }

    public InputSource Source => Bindings[0].Source;

    public bool Active => Bindings.All(binding => binding.Active);

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

    public ComboBinding(params IInputBinding[] bindings)
    {
        Bindings = bindings;
    }
}