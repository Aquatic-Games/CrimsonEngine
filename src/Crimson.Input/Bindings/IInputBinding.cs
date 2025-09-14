using Crimson.Math;

namespace Crimson.Input.Bindings;

public interface IInputBinding
{
    public bool Enabled { get; set; }
    
    public InputSource Source { get; }
    
    public bool IsActive { get; }
    
    public bool BecameActive { get; }
    
    public float Value { get; }
    
    public Vector2T<float> Value2D { get; }

    public void Update();
}