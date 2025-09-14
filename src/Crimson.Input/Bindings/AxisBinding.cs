using Crimson.Math;

namespace Crimson.Input.Bindings;

public struct AxisBinding : IInputBinding
{
    private bool _enabled;
    private InputSource _source;
    private bool _isActive;
    private bool _becameActive;
    private float _value;
    private Vector2T<float> _value2D;
    
    public IInputBinding Positive;

    public IInputBinding Negative;

    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            Positive.Enabled = value;
            Negative.Enabled = value;
        }
    }

    public InputSource Source => _source;

    public bool IsActive => _isActive;

    public bool BecameActive => _becameActive;

    public float Value => _value;

    public Vector2T<float> Value2D => _value2D;

    public AxisBinding(IInputBinding positive, IInputBinding negative)
    {
        Positive = positive;
        Negative = negative;
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
        
        Positive.Update();
        Negative.Update();
        // Cache the results here so that the results aren't calculated multiple times per frame.
        // TODO: Not Ideal.
        _source = Positive.Source;
        _isActive = Positive.IsActive || Negative.IsActive;
        _becameActive = Positive.BecameActive || Negative.BecameActive;
        _value = Positive.Value - Negative.Value;
        _value2D = new Vector2T<float>(_value);
    }
}