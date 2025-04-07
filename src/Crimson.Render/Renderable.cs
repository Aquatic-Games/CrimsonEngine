using Vortice.Direct3D11;

namespace Crimson.Render;

public class Renderable : IDisposable
{
    internal readonly ID3D11Buffer VertexBuffer;
    
    internal readonly ID3D11Buffer IndexBuffer;

    internal readonly uint NumIndices;

    public Material Material;

    public Renderable(Mesh mesh)
    {
        ID3D11Device device = Graphics.Device;

        VertexBuffer = device.CreateBuffer(mesh.Vertices, BindFlags.VertexBuffer);
        IndexBuffer = device.CreateBuffer(mesh.Indices, BindFlags.IndexBuffer);
        
        NumIndices = (uint) mesh.Indices.Length;

        Material = mesh.Material;
    }
    
    public void Dispose()
    {
        IndexBuffer.Dispose();
        VertexBuffer.Dispose();
    }
}