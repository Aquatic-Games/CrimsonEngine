using System.Diagnostics;
using System.Numerics;
using Crimson.Math;
using Crimson.Render.Renderers.Structs;
using Crimson.Render.Utils;
using grabs.Graphics;
using Buffer = grabs.Graphics.Buffer;

namespace Crimson.Render.Renderers;

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

    private readonly DescriptorLayout _textureDescriptor;
    
    private readonly Pipeline _pipeline;

    private readonly List<Draw> _drawQueue;
    
    public TextureBatcher(Device device, Format pixelFormat)
    {
        _vertices = new Vertex[MaxVertices];
        _indices = new uint[MaxIndices];

        _vertexBuffer = device.CreateBuffer(new BufferInfo(BufferUsage.Vertex | BufferUsage.Dynamic,
            MaxVertices * Vertex.SizeInBytes));

        _indexBuffer =
            device.CreateBuffer(new BufferInfo(BufferUsage.Index | BufferUsage.Dynamic,
                MaxIndices * sizeof(uint)));

        _textureDescriptor =
            device.CreateDescriptorLayout(
                new DescriptorLayoutInfo(new DescriptorBinding(0, DescriptorType.Texture, ShaderStage.Pixel)));
        
        ShaderUtils.LoadGraphicsShader(device, "Texture", out ShaderModule? vtxModule, out ShaderModule? pxlModule);
        Debug.Assert(vtxModule != null);
        Debug.Assert(pxlModule != null);

        PipelineInfo pipelineInfo = new()
        {
            VertexShader = vtxModule,
            PixelShader = pxlModule,
            InputLayout =
            [
                new InputElement(Format.R32G32_Float, 0, 0), // Position
                new InputElement(Format.R32G32_Float, 8, 0), // TexCoord
                new InputElement(Format.R32G32B32A32_Float, 16, 0) // Tint
            ],
            ColorAttachmentFormats = [pixelFormat],
            Descriptors = [_textureDescriptor],
            Constants = [new ConstantLayout(ShaderStage.Vertex, 0, CameraMatrices.SizeInBytes)]
        };

        _pipeline = device.CreatePipeline(in pipelineInfo);

        _drawQueue = [];
    }

    public void AddToDrawQueue(in Draw draw)
    {
        _drawQueue.Add(draw);
    }

    public void DispatchDrawQueue(CommandList cl, Matrix4x4 projection, Matrix4x4 transform)
    {
        CameraMatrices matrices = new CameraMatrices(projection, transform);
        cl.PushConstant(_pipeline, ShaderStage.Vertex, 0, matrices);
        
        cl.SetPipeline(_pipeline);
        
        cl.SetVertexBuffer(0, _vertexBuffer, Vertex.SizeInBytes);
        cl.SetIndexBuffer(_indexBuffer, Format.R32_UInt);

        uint numDraws = 0;
        Texture? texture = null;

        foreach (Draw draw in _drawQueue)
        {
            if (draw.Texture != texture || numDraws >= MaxBatches)
            {
                Flush(cl, numDraws, texture);
                numDraws = 0;
            }

            texture = draw.Texture;

            uint vOffset = numDraws * NumVertices;
            uint iOffset = numDraws * NumIndices;

            _vertices[vOffset + 0] = new Vertex(draw.TopLeft, new Vector2(0, 0), Vector4.One);
            _vertices[vOffset + 1] = new Vertex(draw.TopRight, new Vector2(1, 0), Vector4.One);
            _vertices[vOffset + 2] = new Vertex(draw.BottomRight, new Vector2(1, 1), Vector4.One);
            _vertices[vOffset + 3] = new Vertex(draw.BottomLeft, new Vector2(0, 1), Vector4.One);

            _indices[iOffset + 0] = 0 + vOffset;
            _indices[iOffset + 1] = 1 + vOffset;
            _indices[iOffset + 2] = 3 + vOffset;
            _indices[iOffset + 3] = 1 + vOffset;
            _indices[iOffset + 4] = 2 + vOffset;
            _indices[iOffset + 5] = 3 + vOffset;

            numDraws++;
        }
        
        Flush(cl, numDraws, texture);
        _drawQueue.Clear();
    }

    private void Flush(CommandList cl, uint numDraws, Texture? texture)
    {
        // Don't bother drawing unless there's stuff to draw.
        if (numDraws == 0)
            return;
        
        Debug.Assert(texture != null);

        cl.UpdateBuffer<Vertex>(_vertexBuffer,
            _vertices.AsSpan(0, (int) (numDraws * NumVertices * Vertex.SizeInBytes)));
        
        cl.UpdateBuffer<uint>(_indexBuffer, _indices.AsSpan(0, (int) (numDraws * NumIndices * sizeof(uint))));

        cl.PushDescriptor(0, _pipeline, new Descriptor(0, DescriptorType.Texture, texture: texture.TextureHandle));
        
        cl.DrawIndexed(numDraws * NumIndices);
    }
    
    public void Dispose()
    {
        _pipeline.Dispose();
        _textureDescriptor.Dispose();
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

    public readonly struct Draw
    {
        public readonly Texture Texture;
        public readonly Vector2 TopLeft;
        public readonly Vector2 TopRight;
        public readonly Vector2 BottomLeft;
        public readonly Vector2 BottomRight;

        public Draw(Texture texture, Vector2 topLeft, Vector2 topRight, Vector2 bottomLeft, Vector2 bottomRight)
        {
            Texture = texture;
            TopLeft = topLeft;
            TopRight = topRight;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
        }
    }
}