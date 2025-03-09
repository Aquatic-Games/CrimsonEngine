using System.Numerics;
using grabs.Graphics;
using Buffer = grabs.Graphics.Buffer;

namespace Euphoria.Render.Renderers;

/// <summary>
/// Batches textures together efficiently for rendering.
/// </summary>
internal class TextureBatcher : IDisposable
{
    public const uint MaxBatches = 4096;

    private const uint NumVertices = 4;
    private const uint NumIndices = 6;
    
    private const uint MaxVertices = NumVertices * MaxBatches;
    private const uint MaxIndices = NumIndices * MaxBatches;

    private readonly Vertex[] _vertices;
    private readonly uint[] _indices;

    private readonly Buffer _vertexBuffer;
    private readonly Buffer _indexBuffer;

    private readonly MappedData _vMap;
    private readonly MappedData _iMap;
    
    public TextureBatcher(Device device)
    {
        _vertices = new Vertex[MaxVertices];
        _indices = new uint[MaxIndices];
        
        _vertexBuffer = device.CreateBuffer(new BufferInfo(BufferType.Vertex, MaxVertices * Vertex.SizeInBytes, BufferUsage.Dynamic));
        _indexBuffer = device.CreateBuffer(new BufferInfo(BufferType.Index, MaxIndices * sizeof(uint), BufferUsage.Dynamic));

        _vMap = device.MapResource(_vertexBuffer, MapMode.Write);
        _iMap = device.MapResource(_indexBuffer, MapMode.Write);
        
        
    }
    
    public void Dispose()
    {
        _indexBuffer.Dispose();
        _vertexBuffer.Dispose();
    }

    private readonly struct Vertex
    {
        public const uint SizeInBytes = 32;
        
        public readonly Vector2 Position;
        public readonly Vector2 TexCoord;
        public readonly Vector4 Tint; // TODO: Color struct

        public Vertex(Vector2 position, Vector2 texCoord, Vector4 tint)
        {
            Position = position;
            TexCoord = texCoord;
            Tint = tint;
        }
    }
}