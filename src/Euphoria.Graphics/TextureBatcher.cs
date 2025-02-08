using System.Numerics;
using Euphoria.Math;
using SDL;
using static Euphoria.Graphics.SdlUtil;
using static SDL.SDL3;

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

    private readonly SDL_GPUTransferBuffer* _transferBuffer;

    public TextureBatcher(SDL_GPUDevice* device)
    {
        _device = device;

        _vertices = new Vertex[MaxVertices];
        _indices = new ushort[MaxIndices];

        SDL_GPUBufferCreateInfo vertexBufferInfo = new SDL_GPUBufferCreateInfo()
        {
            size = (uint) (MaxVertices * sizeof(Vertex)),
            usage = SDL_GPUBufferUsageFlags.SDL_GPU_BUFFERUSAGE_VERTEX
        };

        _vertexBuffer = Check(SDL_CreateGPUBuffer(_device, &vertexBufferInfo), "Create vertex buffer");

        SDL_GPUBufferCreateInfo indexBufferInfo = new SDL_GPUBufferCreateInfo()
        {
            size = MaxIndices * sizeof(ushort),
            usage = SDL_GPUBufferUsageFlags.SDL_GPU_BUFFERUSAGE_INDEX
        };

        _indexBuffer = Check(SDL_CreateGPUBuffer(_device, &indexBufferInfo), "Create index buffer");

        SDL_GPUTransferBufferCreateInfo transferBufferInfo = new SDL_GPUTransferBufferCreateInfo()
        {
            size = vertexBufferInfo.size + indexBufferInfo.size,
            usage = SDL_GPUTransferBufferUsage.SDL_GPU_TRANSFERBUFFERUSAGE_UPLOAD
        };

        _transferBuffer = Check(SDL_CreateGPUTransferBuffer(_device, &transferBufferInfo), "Create transfer buffer");
    }
    
    public void Dispose()
    {
        SDL_ReleaseGPUTransferBuffer(_device, _transferBuffer);
        SDL_ReleaseGPUBuffer(_device, _indexBuffer);
        SDL_ReleaseGPUBuffer(_device, _vertexBuffer);
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