using System.Numerics;
using Crimson.Math;

namespace Crimson.Render;

/// <summary>
/// The primary vertex type that is passed into shaders.
/// </summary>
/// <param name="Position">The model-space position.</param>
/// <param name="TexCoord">The texture coordinate.</param>
/// <param name="Color">The vertex color.</param>
/// <param name="Normal">The normal vector.</param>
public record struct Vertex(Vector3 Position, Vector2 TexCoord, Color Color, Vector3 Normal)
{
    public const uint SizeInBytes = 48;
    
    /// <summary>
    /// The model-space position.
    /// </summary>
    public Vector3 Position = Position;
    
    /// <summary>
    /// The texture coordinate.
    /// </summary>
    public Vector2 TexCoord = TexCoord;
    
    /// <summary>
    /// The vertex color.
    /// </summary>
    public Color Color = Color;
    
    /// <summary>
    /// The normal vector.
    /// </summary>
    public Vector3 Normal = Normal;
}