using System.Numerics;
using Crimson.Graphics.Renderers.Structs;
using Crimson.Graphics.Utils;
using Crimson.Math;
using Graphite;
using Buffer = Graphite.Buffer;
using IndexType = uint;

namespace Crimson.Graphics.Renderers;

internal sealed class CanvasRenderer : IDisposable
{
    private readonly Device _device;

    private Vertex[] _vertices;
    private IndexType[] _indices;

    private Buffer _vertexBuffer;
    private Buffer _indexBuffer;

    private readonly Buffer _cameraBuffer;
    
    private readonly DescriptorLayout _cameraLayout;
    private readonly DescriptorLayout _textureLayout;
    private readonly DescriptorSet _cameraSet;

    private readonly Pipeline _pipeline;

    public CanvasRenderer(Device device, Format outFormat)
    {
        _device = device;

        _vertices = new Vertex[4096 * 4];
        _indices = new IndexType[4096 * 6];

        _vertexBuffer = _device.CreateBuffer(new BufferInfo(BufferUsage.VertexBuffer | BufferUsage.MapWrite,
            (uint) (_vertices.Length * Vertex.SizeInBytes)));
        _indexBuffer = _device.CreateBuffer(new BufferInfo(BufferUsage.IndexBuffer | BufferUsage.MapWrite,
            (uint) (_indices.Length * sizeof(IndexType))));

        _cameraBuffer = _device.CreateBuffer(BufferUsage.ConstantBuffer | BufferUsage.MapWrite,
            new CameraMatrices(Matrix<float>.Identity, Matrix<float>.Identity));

        _cameraLayout = _device.CreateDescriptorLayout(new DescriptorLayoutInfo
        {
            Bindings = [new DescriptorBinding(0, DescriptorType.ConstantBuffer, ShaderStage.Vertex)]
        });

        _textureLayout = _device.CreateDescriptorLayout(new DescriptorLayoutInfo
        {
            Bindings = [new DescriptorBinding(0, DescriptorType.Texture, ShaderStage.Pixel)],
            PushDescriptor = true
        });

        _cameraSet = _device.CreateDescriptorSet(_cameraLayout,
            new Descriptor(0, DescriptorType.ConstantBuffer, _cameraBuffer));

        ShaderUtils.LoadGraphicsShader(_device, "Canvas/Texture", out ShaderModule vtxShader,
            out ShaderModule pxlShader);

        GraphicsPipelineInfo pipelineInfo = new()
        {
            VertexShader = vtxShader,
            PixelShader = pxlShader,
            ColorTargets = [new ColorTargetInfo(outFormat)],
            InputLayout =
            [
                new InputElementDescription(Format.R32G32_Float, 0, 0, 0),
                new InputElementDescription(Format.R32G32_Float, 8, 1, 0),
                new InputElementDescription(Format.R32G32B32A32_Float, 16, 2, 0)
            ],
            Descriptors = [_cameraLayout, _textureLayout]
        };

        _pipeline = _device.CreateGraphicsPipeline(in pipelineInfo);
    }
    
    public void Dispose()
    {
        _pipeline.Dispose();
        _cameraSet.Dispose();
        _textureLayout.Dispose();
        _cameraLayout.Dispose();
        _indexBuffer.Dispose();
        _vertexBuffer.Dispose();
    }

    private struct Vertex
    {
        public const uint SizeInBytes = 32;
        
        public Vector2T<int> Position;
        public Vector2T<int> TexCoord;
        public Color Tint;

        public Vertex(Vector2T<int> position, Vector2T<int> texCoord, Color tint)
        {
            Position = position;
            TexCoord = texCoord;
            Tint = tint;
        }
    }
}