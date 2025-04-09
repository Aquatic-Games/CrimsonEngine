using Crimson.Render.Primitives;

namespace Crimson.Render;

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