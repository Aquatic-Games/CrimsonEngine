using Crimson.Math;

namespace Crimson.Input.Bindings;

public struct DualAxisBinding : IInputBinding
{
    private bool _enabled;
    private InputSource _source;
    private bool _isActive;
    private bool _becameActive;
    private Vector2T<float> _value2D;
    private float _value;
    
    public IInputBinding Up;

    public IInputBinding Down;

    public IInputBinding Right;
    
    public IInputBinding Left;

    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            Up.Enabled = value;
            Down.Enabled = value;
            Right.Enabled = value;
            Left.Enabled = value;
        }
    }
    
    public InputSource Source => _source;

    public bool IsActive => _isActive;

    public bool BecameActive => _becameActive;

    public float Value => _value;

    public Vector2T<float> Value2D => _value2D;

    public DualAxisBinding(IInputBinding up, IInputBinding down, IInputBinding right, IInputBinding left)
    {
        Up = up;
        Down = down;
        Right = right;
        Left = left;
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
        Right.Update();
        Left.Update();

        // TODO: Not ideal.
        _source = Up.Source;
        _isActive = Up.IsActive || Down.IsActive || Right.IsActive || Left.IsActive;
        _becameActive = Up.BecameActive || Down.BecameActive || Right.BecameActive || Left.BecameActive;
        _value2D = new Vector2T<float>(Right.Value - Left.Value, Up.Value - Down.Value);
        _value = Vector2T.Magnitude(_value2D);
    }
}