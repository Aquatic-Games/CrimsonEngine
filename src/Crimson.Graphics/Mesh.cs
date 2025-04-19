using Crimson.Graphics.Materials;
using Crimson.Graphics.Primitives;

namespace Crimson.Graphics;

/// <summary>
/// A mesh contains vertices, indices, and a material, that can be drawn to the screen.
/// </summary>
public class Mesh
{
    public Vertex[] Vertices;

    public uint[] Indices;

    public Material Material;

    public Mesh(Vertex[] vertices, uint[] indices, Material material)
    {
        Vertices = vertices;
        Indices = indices;
        Material = material;
    }

    public static Mesh FromPrimitive(IPrimitive primitive, Material material)
    {
        return new Mesh(primitive.Vertices, primitive.Indices, material);
    }
}