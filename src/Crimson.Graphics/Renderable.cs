using Crimson.Graphics.Materials;
using Vortice.Direct3D11;

namespace Crimson.Graphics;

/// <summary>
/// A renderable is a GPU object that can be drawn to the screen. All rendering done by the main 3D renderer will use a
/// renderable.
/// </summary>
public class Renderable : IDisposable
{
    internal readonly ID3D11Buffer VertexBuffer;
    
    internal readonly ID3D11Buffer IndexBuffer;

    internal readonly uint NumIndices;

    /// <summary>
    /// The <see cref="Materials.Material"/> of this renderable.
    /// </summary>
    public Material Material;

    /// <summary>
    /// Create a <see cref="Renderable"/> that can be drawn.
    /// </summary>
    /// <param name="renderer">A <see cref="Renderer"/> instance.</param>
    /// <param name="mesh">The mesh to use.</param>
    public Renderable(Renderer renderer, Mesh mesh)
    {
        ID3D11Device device = renderer.Device;
        
        VertexBuffer = device.CreateBuffer(mesh.Vertices, BindFlags.VertexBuffer);
        IndexBuffer = device.CreateBuffer(mesh.Indices, BindFlags.IndexBuffer);
        
        NumIndices = (uint) mesh.Indices.Length;

        Material = mesh.Material;
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