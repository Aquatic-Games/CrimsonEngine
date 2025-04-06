namespace Euphoria.Render;

public class Mesh
{
    public Vertex[] Vertices;

    public uint[] Indices;

    public Mesh(Vertex[] vertices, uint[] indices)
    {
        Vertices = vertices;
        Indices = indices;
    }
}