using Crimson.Math;

namespace Crimson.Graphics;

/// <summary>
/// The primary vertex type that is passed into shaders.
/// </summary>
public struct Vertex
{
    public const uint SizeInBytes = 48;
    
    /// <summary>
    /// The model-space position.
    /// </summary>
    public Vector3T<float> Position;
    
    /// <summary>
    /// The texture coordinate.
    /// </summary>
    public Vector2T<float> TexCoord;
    
    /// <summary>
    /// The vertex color.
    /// </summary>
    public Color Color;
    
    /// <summary>
    /// The normal vector.
    /// </summary>
    public Vector3T<float> Normal;

    /// <summary>
    /// Create a new <see cref="Vertex"/>.
    /// </summary>
    /// <param name="position">The model-space position.</param>
    /// <param name="texCoord">The texture coordinate.</param>
    /// <param name="color">The vertex color.</param>
    /// <param name="normal">The normal vector.</param>
    public Vertex(Vector3T<float> position, Vector2T<float> texCoord, Color color, Vector3T<float> normal)
    {
        Position = position;
        TexCoord = texCoord;
        Color = color;
        Normal = normal;
    }
}