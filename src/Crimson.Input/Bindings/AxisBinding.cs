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
    
    public IInputBinding Up;

    public IInputBinding Down;

    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            Up.Enabled = value;
            Down.Enabled = value;
        }
    }

    public InputSource Source => _source;

    public bool IsActive => _isActive;

    public bool BecameActive => _becameActive;

    public float Value => _value;

    public Vector2T<float> Value2D => _value2D;

    public AxisBinding(IInputBinding up, IInputBinding down)
    {
        Up = up;
        Down = down;
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
        
        Up.Update();
        Down.Update();
        // Cache the results here so that the results aren't calculated multiple times per frame.
        // TODO: Not Ideal.
        _source = Up.Source;
        _isActive = Up.IsActive || Down.IsActive;
        _becameActive = Up.BecameActive || Down.BecameActive;
        _value = Up.Value - Down.Value;
        _value2D = new Vector2T<float>(_value);
    }
}