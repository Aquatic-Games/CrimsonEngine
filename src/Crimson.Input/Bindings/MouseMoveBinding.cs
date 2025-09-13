using Crimson.Math;

namespace Crimson.Input.Bindings;

public class MouseMoveBinding : IInputBinding
{
    public bool Enabled { get; set; }

    public InputSource Source => InputSource.Absolute;
    
    public bool Active => Value2D != Vector2T<float>.Zero;

    public float Value => Vector2T.Magnitude(Value2D);

    public Vector2T<float> Value2D => Enabled ? Input.MouseDelta : Vector2T<float>.Zero;
}