using Crimson.Graphics.Materials;
using Graphite;
using Buffer = Graphite.Buffer;

namespace Crimson.Graphics;

/// <summary>
/// A renderable is a GPU object that can be drawn to the screen. All rendering done by the main 3D renderer will use a
/// renderable.
/// </summary>
public class Renderable : IDisposable
{
    internal readonly Buffer VertexBuffer;
    
    internal readonly Buffer IndexBuffer;

    internal readonly uint NumIndices;

    /// <summary>
    /// The <see cref="Materials.Material"/> of this renderable.
    /// </summary>
    public Material Material;

    /// <summary>
    /// Create a <see cref="Renderable"/> that can be drawn.
    /// </summary>
    /// <param name="mesh">The mesh to use.</param>
    public Renderable(Mesh mesh)
    {
        // TODO: Look into replacing renderables with just the mesh class, so the Mesh class will contain the vertex and index buffers.
        
        Device device = Renderer.Device;

        VertexBuffer = device.CreateBuffer(BufferUsage.VertexBuffer, mesh.Vertices);
        IndexBuffer = device.CreateBuffer(BufferUsage.IndexBuffer, mesh.Indices);
        
        NumIndices = (uint) mesh.Indices.Length;

        Material = mesh.Material;
    }

    public Renderable(ReadOnlySpan<Vertex> vertices, ReadOnlySpan<uint> indices, Material material)
    {
        Device device = Renderer.Device;

        VertexBuffer = device.CreateBuffer(BufferUsage.VertexBuffer, vertices);
        IndexBuffer = device.CreateBuffer(BufferUsage.IndexBuffer, indices);

        NumIndices = (uint) indices.Length;
        Material = material;
    }
    
    /// <summary>
    /// Dispose of this <see cref="Renderable"/>.
    /// </summary>
    public void Dispose()
    {
        IndexBuffer.Dispose();
        VertexBuffer.Dispose();
    }
}