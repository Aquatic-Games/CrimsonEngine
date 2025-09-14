using Crimson.Math;

namespace Crimson.Input.Bindings;

public struct ComboBinding : IInputBinding
{
    private bool _enabled;
    private InputSource _source;
    private bool _isActive;
    private bool _becameActive;
    private float _value;
    private Vector2T<float> _value2D;
    
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

    public InputSource Source => _source;

    public bool IsActive => _isActive;

    public bool BecameActive => _becameActive;

    public float Value => _value;

    public Vector2T<float> Value2D => _value2D;

    public ComboBinding(params IInputBinding[] bindings)
    {
        Bindings = bindings;
    }
    
    public void Update()
    {
        if (!_enabled)
        {
            _isActive = false;
            _becameActive = false;
            _value = 0;
            _value2D = Vector2T<float>.Zero;
            return;
        }

        _source = Bindings[0].Source;
        _isActive = Bindings.All(binding => binding.IsActive);
        _becameActive = Bindings.All(binding => binding.BecameActive);
        _value = Bindings.Sum(binding => binding.Value);
        
        _value2D = Vector2T<float>.Zero;
        foreach (IInputBinding binding in Bindings)
            _value2D += binding.Value2D;
    }
}