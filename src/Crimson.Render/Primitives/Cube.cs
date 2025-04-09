using System.Numerics;
using Crimson.Math;

namespace Crimson.Render.Primitives;

/// <summary>
/// A 3D cube primitive.
/// </summary>
public class Cube : IPrimitive
{
    /// <summary>
    /// The cube's vertices.
    /// </summary>
    public Vertex[] Vertices { get; }
    
    /// <summary>
    /// The cube's indices.
    /// </summary>
    public uint[] Indices { get; }

    /// <summary>
    /// Create a new <see cref="Cube"/> at the default size (1 unit).
    /// </summary>
    public Cube()
    {
        Vertices =
        [
            new Vertex(new Vector3(-0.5f, 0.5f, -0.5f), new Vector2(0, 0), Color.White, new Vector3(0, 1, 0)),
            new Vertex(new Vector3(0.5f, 0.5f, -0.5f), new Vector2(1, 0), Color.White, new Vector3(0, 1, 0)),
            new Vertex(new Vector3(0.5f, 0.5f, 0.5f), new Vector2(1, 1), Color.White, new Vector3(0, 1, 0)),
            new Vertex(new Vector3(-0.5f, 0.5f, 0.5f), new Vector2(0, 1), Color.White, new Vector3(0, 1, 0)),

            new Vertex(new Vector3(-0.5f, -0.5f, 0.5f), new Vector2(0, 0), Color.White, new Vector3(0, -1, 0)),
            new Vertex(new Vector3(0.5f, -0.5f, 0.5f), new Vector2(1, 0), Color.White, new Vector3(0, -1, 0)),
            new Vertex(new Vector3(0.5f, -0.5f, -0.5f), new Vector2(1, 1), Color.White, new Vector3(0, -1, 0)),
            new Vertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(0, 1), Color.White, new Vector3(0, -1, 0)),

            new Vertex(new Vector3(-0.5f, 0.5f, -0.5f), new Vector2(0, 0), Color.White, new Vector3(-1, 0, 0)),
            new Vertex(new Vector3(-0.5f, 0.5f, 0.5f), new Vector2(1, 0), Color.White, new Vector3(-1, 0, 0)),
            new Vertex(new Vector3(-0.5f, -0.5f, 0.5f), new Vector2(1, 1), Color.White, new Vector3(-1, 0, 0)),
            new Vertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(0, 1), Color.White, new Vector3(-1, 0, 0)),

            new Vertex(new Vector3(0.5f, 0.5f, 0.5f), new Vector2(0, 0), Color.White, new Vector3(1, 0, 0)),
            new Vertex(new Vector3(0.5f, 0.5f, -0.5f), new Vector2(1, 0), Color.White, new Vector3(1, 0, 0)),
            new Vertex(new Vector3(0.5f, -0.5f, -0.5f), new Vector2(1, 1), Color.White, new Vector3(1, 0, 0)),
            new Vertex(new Vector3(0.5f, -0.5f, 0.5f), new Vector2(0, 1), Color.White, new Vector3(1, 0, 0)),

            new Vertex(new Vector3(0.5f, 0.5f, -0.5f), new Vector2(0, 0), Color.White, new Vector3(0, 0, -1)),
            new Vertex(new Vector3(-0.5f, 0.5f, -0.5f), new Vector2(1, 0), Color.White, new Vector3(0, 0, -1)),
            new Vertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(1, 1), Color.White, new Vector3(0, 0, -1)),
            new Vertex(new Vector3(0.5f, -0.5f, -0.5f), new Vector2(0, 1), Color.White, new Vector3(0, 0, -1)),

            new Vertex(new Vector3(-0.5f, 0.5f, 0.5f), new Vector2(0, 0), Color.White, new Vector3(0, 0, 1)),
            new Vertex(new Vector3(0.5f, 0.5f, 0.5f), new Vector2(1, 0), Color.White, new Vector3(0, 0, 1)),
            new Vertex(new Vector3(0.5f, -0.5f, 0.5f), new Vector2(1, 1), Color.White, new Vector3(0, 0, 1)),
            new Vertex(new Vector3(-0.5f, -0.5f, 0.5f), new Vector2(0, 1), Color.White, new Vector3(0, 0, 1)),
        ];

        Indices =
        [
            0, 1, 2, 0, 2, 3,
            4, 5, 6, 4, 6, 7,
            8, 9, 10, 8, 10, 11,
            12, 13, 14, 12, 14, 15,
            16, 17, 18, 16, 18, 19,
            20, 21, 22, 20, 22, 23
        ];
    }
}