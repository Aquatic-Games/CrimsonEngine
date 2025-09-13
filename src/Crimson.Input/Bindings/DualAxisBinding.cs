using Crimson.Math;

namespace Crimson.Input.Bindings;

public struct DualAxisBinding : IInputBinding
{
    private bool _enabled;
    
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

    // TODO: Not ideal.
    public InputSource Source => Up.Source;
    
    public bool Active => (Up.Active || Down.Active || Right.Active || Left.Active);

    public float Value => Vector2T.Magnitude(Value2D);

    public Vector2T<float> Value2D => new Vector2T<float>(Right.Value - Left.Value, Up.Value - Down.Value);
    
    public DualAxisBinding(IInputBinding up, IInputBinding down, IInputBinding right, IInputBinding left)
    {
        Up = up;
        Down = down;
        Right = right;
        Left = left;
    }
}