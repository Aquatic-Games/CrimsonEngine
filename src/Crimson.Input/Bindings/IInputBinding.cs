using Crimson.Math;

namespace Crimson.Input.Bindings;

public interface IInputBinding
{
    public bool Enabled { get; internal set; }
    
    public bool Pressed { get; }
    
    public float Value { get; }
    
    public Vector2T<float> Value2D { get; }
}