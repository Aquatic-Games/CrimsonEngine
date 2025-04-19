namespace Crimson.Graphics.Primitives;

/// <summary>
/// Represents a primitive shape.
/// </summary>
public interface IPrimitive
{
    /// <summary>
    /// The primitive's vertices.
    /// </summary>
    public Vertex[] Vertices { get; }
    
    /// <summary>
    /// The primitive's indices.
    /// </summary>
    public uint[] Indices { get; }
}