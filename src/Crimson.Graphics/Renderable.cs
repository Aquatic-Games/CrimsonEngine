using Crimson.Graphics.Materials;
using Crimson.Graphics.Utils;
using SDL3;

namespace Crimson.Graphics;

/// <summary>
/// A renderable is a GPU object that can be drawn to the screen. All rendering done by the main 3D renderer will use a
/// renderable.
/// </summary>
public class Renderable : IDisposable
{
    private readonly IntPtr _device;
    
    internal readonly IntPtr VertexBuffer;
    
    internal readonly IntPtr IndexBuffer;

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
        _device = Renderer.Device;

        VertexBuffer = SdlUtils.CreateBuffer(_device, SDL.GPUBufferUsageFlags.Vertex, mesh.Vertices);
        IndexBuffer = SdlUtils.CreateBuffer(_device, SDL.GPUBufferUsageFlags.Index, mesh.Indices);
        
        NumIndices = (uint) mesh.Indices.Length;

        Material = mesh.Material;
    }
    
    /// <summary>
    /// Dispose of this <see cref="Renderable"/>.
    /// </summary>
    public void Dispose()
    {
        SDL.ReleaseGPUBuffer(_device, IndexBuffer);
        SDL.ReleaseGPUBuffer(_device, VertexBuffer);
    }
}