using Crimson.Math;

namespace Crimson.Input.Bindings;

public struct AxisBinding : IInputBinding
{
    private bool _enabled;
    
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

    public InputSource Source => Up.Source;
    
    public bool Active => Up.Active || Down.Active;

    public float Value => Up.Value - Down.Value;

    public Vector2T<float> Value2D => new Vector2T<float>(Value);
    
    public AxisBinding(IInputBinding up, IInputBinding down)
    {
        Up = up;
        Down = down;
    }
}