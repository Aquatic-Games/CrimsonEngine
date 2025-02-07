using System.Numerics;
using Euphoria.Math;
using SDL;

namespace Euphoria.Graphics;

public unsafe class TextureBatcher : IDisposable
{
    public const uint MaxBatches = 4096;

    private const uint NumVertices = 4;
    private const uint NumIndices = 6;

    private const uint MaxVertices = NumVertices * MaxBatches;
    private const uint MaxIndices = NumIndices * MaxBatches;
    
    private readonly SDL_GPUDevice* _device;

    private readonly Vertex[] _vertices;
    private readonly ushort[] _indices;

    private readonly SDL_GPUBuffer* _vertexBuffer;
    private readonly SDL_GPUBuffer* _indexBuffer;

    public TextureBatcher(SDL_GPUDevice* device)
    {
        _device = device;

        _vertices = new Vertex[MaxVertices];
        _indices = new ushort[MaxIndices];
        
        
    }
    
    public void Dispose()
    {
        
    }

    private readonly struct Vertex
    {
        public readonly Vector2 Position;

        public readonly Vector2 TexCoord;

        public readonly Color Tint;

        public Vertex(Vector2 position, Vector2 texCoord, Color tint)
        {
            Position = position;
            TexCoord = texCoord;
            Tint = tint;
        }
    }
}