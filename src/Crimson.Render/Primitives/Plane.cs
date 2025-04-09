using System.Numerics;
using Crimson.Math;

namespace Crimson.Render.Primitives;

/// <summary>
/// A flat, 2D plane with no depth.
/// </summary>
public class Plane : IPrimitive
{
    /// <summary>
    /// The plane's vertices.
    /// </summary>
    public Vertex[] Vertices { get; }
    
    /// <summary>
    /// The plane's indices.
    /// </summary>
    public uint[] Indices { get; }

    // TODO: Subdivisions
    /// <summary>
    /// Create a <see cref="Plane"/> with no subdivisions.
    /// </summary>
    public Plane()
    {
        Vertices =
        [
            new Vertex(new Vector3(-0.5f, +0.5f, 0.0f), new Vector2(0, 0), new Color(1.0f, 1.0f, 1.0f), new Vector3(0, 0, 1)),
            new Vertex(new Vector3(+0.5f, +0.5f, 0.0f), new Vector2(1, 0), new Color(1.0f, 1.0f, 1.0f), new Vector3(0, 0, 1)),
            new Vertex(new Vector3(+0.5f, -0.5f, 0.0f), new Vector2(1, 1), new Color(1.0f, 1.0f, 1.0f), new Vector3(0, 0, 1)),
            new Vertex(new Vector3(-0.5f, -0.5f, 0.0f), new Vector2(0, 1), new Color(1.0f, 1.0f, 1.0f), new Vector3(0, 0, 1)),
        ];

        Indices =
        [
            0, 1, 3,
            1, 2, 3
        ];
    }
}