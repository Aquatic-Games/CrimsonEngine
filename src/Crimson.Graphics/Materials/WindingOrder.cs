namespace Crimson.Graphics.Materials;

/// <summary>
/// Defines the winding order of a mesh.
/// </summary>
public enum WindingOrder
{
    /// <summary>
    /// Counter-clockwise winding order. This is the default and should be preferred when possible.
    /// </summary>
    CounterClockwise,
    
    /// <summary>
    /// Clockwise winding order.
    /// </summary>
    Clockwise
}