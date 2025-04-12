using Crimson.Render.Materials;
using Crimson.Render.Primitives;

namespace Crimson.Render;

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