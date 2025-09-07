using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Crimson.Graphics.Renderers.Structs;
using Crimson.Graphics.Utils;
using Crimson.Math;
using Graphite;
using Graphite.Core;
using Buffer = Graphite.Buffer;

namespace Crimson.Graphics.Renderers;

/// <summary>
/// Batches textures together efficiently for rendering.
/// </summary>
internal class TextureBatcher : IDisposable
{
    private const uint NumVertices = 4;
    private const uint NumIndices = 6;

    private readonly Device _device;

    private Vertex[] _vertices;
    private uint[] _indices;

    private uint _vBufferSize;
    private uint _iBufferSize;
    
    private Buffer _vertexBuffer;
    private Buffer _indexBuffer;

    private Buffer _cameraBuffer;
    private DescriptorLayout _cameraBufferLayout;
    private DescriptorSet _cameraBufferSet;

    private DescriptorLayout _textureLayout;

    //private readonly IntPtr _blendPipeline;
    private readonly Pipeline _noBlendPipeline;

    private readonly Sampler _sampler;

    private readonly List<Draw> _drawQueue;
    private readonly List<DrawList> _drawList;
    
    public unsafe TextureBatcher(Device device, Format format)
    {
        _device = device;

        // Create with an initial size of 4096 sprites
        const uint initialSize = 4096;

        _vBufferSize = initialSize * NumVertices;
        _iBufferSize = initialSize * NumIndices;
        
        _vertices = new Vertex[_vBufferSize];
        _indices = new uint[_iBufferSize];

        _vertexBuffer = _device.CreateBuffer(new BufferInfo(BufferUsage.VertexBuffer | BufferUsage.MapWrite, _vBufferSize * Vertex.SizeInBytes));
        _indexBuffer = _device.CreateBuffer(new BufferInfo(BufferUsage.IndexBuffer | BufferUsage.MapWrite, _iBufferSize * sizeof(uint)));
        _cameraBuffer = _device.CreateBuffer(new BufferInfo(BufferUsage.ConstantBuffer | BufferUsage.MapWrite, CameraMatrices.SizeInBytes));

        ShaderUtils.LoadGraphicsShader(device, "Texture", out ShaderModule? vertexShader,
            out ShaderModule? pixelShader);

        _cameraBufferLayout = _device.CreateDescriptorLayout(new DescriptorLayoutInfo
        {
            Bindings = [new DescriptorBinding(0, DescriptorType.ConstantBuffer, ShaderStage.Vertex)]
        });
        
        _cameraBufferSet = _device.CreateDescriptorSet(_cameraBufferLayout,
            new Descriptor(0, DescriptorType.ConstantBuffer, buffer: _cameraBuffer));

        _textureLayout = _device.CreateDescriptorLayout(new DescriptorLayoutInfo
        {
            Bindings = [new DescriptorBinding(0, DescriptorType.Texture, ShaderStage.Pixel)],
            PushDescriptor = true
        });

        GraphicsPipelineInfo pipelineInfo = new()
        {
            VertexShader = vertexShader,
            PixelShader = pixelShader,
            ColorTargets = [new ColorTargetInfo(format)],
            InputLayout =
            [
                new InputElementDescription(Format.R32G32_Float, 0, 0, 0),
                new InputElementDescription(Format.R32G32_Float, 8, 1, 0),
                new InputElementDescription(Format.R32G32B32A32_Float, 16, 2, 0)
            ],
            Descriptors = [_cameraBufferLayout, _textureLayout]
        };

        //_blendPipeline = SDL.CreateGPUGraphicsPipeline(device, in pipelineInfo).Check("Create GPU pipeline");
        //targetDesc.BlendState = SdlUtils.NoBlend;
        _noBlendPipeline = _device.CreateGraphicsPipeline(in pipelineInfo);
        
        pixelShader.Dispose();
        vertexShader.Dispose();

        SamplerInfo samplerInfo = new()
        {
            MinFilter = Filter.Linear,
            MagFilter = Filter.Linear,
            MipFilter = Filter.Linear,
            AddressU = AddressMode.Wrap,
            AddressV = AddressMode.Wrap,
            MaxLod = 1000
        };

        _sampler = _device.CreateSampler(in samplerInfo);

        _drawQueue = [];
        _drawList = [];
    }

    public void AddToDrawQueue(in Draw draw)
    {
        _drawQueue.Add(draw);
    }

    public unsafe bool Render(CommandList cl, GrTexture colorTarget, bool shouldClear, Size<int> size, CameraMatrices matrices)
    {
        // Don't even try to draw if the draw count is 0.
        if (_drawQueue.Count == 0)
            return false;
        
        uint numDraws = 0;
        uint bufferOffset = 0;
        uint lastBufferOffset = 0;
        Texture? texture = null;
        BlendMode blendMode = BlendMode.Blend;

        foreach (Draw draw in _drawQueue)
        {
            if ((draw.Texture != texture || draw.Blend != blendMode) && numDraws != 0)
            {
                _drawList.Add(new DrawList(texture, numDraws, lastBufferOffset, blendMode));
                lastBufferOffset = bufferOffset;
                numDraws = 0;
            }

            texture = draw.Texture;
            blendMode = draw.Blend;

            uint vOffset = bufferOffset * NumVertices;
            uint iOffset = bufferOffset * NumIndices;

            Size<float> texSize = texture.Size.As<float>();
            Rectangle<int> source = draw.Source;

            float texX = source.X / texSize.Width;
            float texY = source.Y / texSize.Height;
            float texW = source.Width / texSize.Width;
            float texH = source.Height / texSize.Height;

            _vertices[vOffset + 0] = new Vertex(draw.TopLeft, new Vector2T<float>(texX, texY), draw.Tint);
            _vertices[vOffset + 1] = new Vertex(draw.TopRight, new Vector2T<float>(texX + texW, texY), draw.Tint);
            _vertices[vOffset + 2] = new Vertex(draw.BottomRight, new Vector2T<float>(texX + texW, texY + texH), draw.Tint);
            _vertices[vOffset + 3] = new Vertex(draw.BottomLeft, new Vector2T<float>(texX, texY + texH), draw.Tint);

            _indices[iOffset + 0] = 0 + vOffset;
            _indices[iOffset + 1] = 1 + vOffset;
            _indices[iOffset + 2] = 3 + vOffset;
            _indices[iOffset + 3] = 1 + vOffset;
            _indices[iOffset + 4] = 2 + vOffset;
            _indices[iOffset + 5] = 3 + vOffset;

            numDraws++;
            bufferOffset++;
        }
        
        _drawList.Add(new DrawList(texture, numDraws, lastBufferOffset, blendMode));
        _drawQueue.Clear();
        
        uint numVertexBytes = bufferOffset * NumVertices * Vertex.SizeInBytes;
        uint numIndexBytes = bufferOffset * NumIndices * sizeof(uint);

        void* vMap = (void*) _device.MapBuffer(_vertexBuffer);
        fixed (Vertex* pVertices = _vertices)
            Unsafe.CopyBlock(vMap, pVertices, numVertexBytes);
        _device.UnmapBuffer(_vertexBuffer);

        void* iMap = (void*) _device.MapBuffer(_indexBuffer);
        fixed (uint* pIndices = _indices)
            Unsafe.CopyBlock(iMap, pIndices, numIndexBytes);

        _device.UpdateBuffer(_cameraBuffer, 0, matrices);
        
        //SdlUtils.PushDebugGroup(cb, "TextureBatcher Pass");

        ColorAttachmentInfo colorAttachment = new()
        {
            Texture = colorTarget,
            ClearColor = new ColorF(0, 0, 0),
            LoadOp = shouldClear ? LoadOp.Clear : LoadOp.Load,
            StoreOp = StoreOp.Store
        };

        cl.BeginRenderPass(in colorAttachment);

        /*TODO: SDL.SetGPUViewport(renderPass,
            new SDL.GPUViewport { X = 0, Y = 0, W = size.Width, H = size.Height, MinDepth = 0, MaxDepth = 1 });*/
        
        cl.SetVertexBuffer(0, _vertexBuffer, Vertex.SizeInBytes);
        cl.SetIndexBuffer(_indexBuffer, Format.R32_UInt);
        
        foreach (DrawList drawList in _drawList)
        {
            Flush(cl, in drawList);
        }
        
        cl.EndRenderPass();
        
        //SdlUtils.PopDebugGroup(cb);
        
        _drawList.Clear();

        return true;
    }

    private unsafe void Flush(CommandList cl, ref readonly DrawList drawList)
    {
        Debug.Assert(drawList.NumDraws != 0);
        Debug.Assert(drawList.Texture != null);

        Pipeline pipeline = drawList.Blend switch
        {
            BlendMode.None => _noBlendPipeline,
            BlendMode.Blend => _noBlendPipeline,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        cl.SetGraphicsPipeline(pipeline);
        cl.SetDescriptorSet(0, pipeline, _cameraBufferSet);
        cl.PushDescriptors(1, pipeline,
            new Descriptor(0, DescriptorType.Texture, texture: drawList.Texture.GrTexture, sampler: _sampler));

        cl.DrawIndexed(drawList.NumDraws * NumIndices, drawList.Offset * NumIndices);
    }
    
    public void Dispose()
    {
        _sampler.Dispose();
        _textureLayout.Dispose();
        _cameraBufferSet.Dispose();
        _cameraBufferLayout.Dispose();
        _noBlendPipeline.Dispose();
        _cameraBuffer.Dispose();
        _indexBuffer.Dispose();
        _vertexBuffer.Dispose();
    }

    private readonly struct Vertex
    {
        public const uint SizeInBytes = 32;
        
        public readonly Vector2T<float> Position;
        public readonly Vector2T<float> TexCoord;
        public readonly Color Tint;

        public Vertex(Vector2T<float> position, Vector2T<float> texCoord, Color tint)
        {
            Position = position;
            TexCoord = texCoord;
            Tint = tint;
        }
    }

    private readonly struct DrawList(Texture? texture, uint numDraws, uint offset, BlendMode blend)
    {
        public readonly Texture? Texture = texture;
        public readonly uint NumDraws = numDraws;
        public readonly uint Offset = offset;
        public readonly BlendMode Blend = blend;
    }

    public readonly struct Draw
    {
        public readonly Texture Texture;
        public readonly Vector2T<float> TopLeft;
        public readonly Vector2T<float> TopRight;
        public readonly Vector2T<float> BottomLeft;
        public readonly Vector2T<float> BottomRight;
        public readonly Rectangle<int> Source;
        public readonly Color Tint;
        public readonly BlendMode Blend;

        public Draw(Texture texture, Vector2T<float> topLeft, Vector2T<float> topRight, Vector2T<float> bottomLeft,
            Vector2T<float> bottomRight, Rectangle<int> source, Color tint, BlendMode blend)
        {
            Texture = texture;
            TopLeft = topLeft;
            TopRight = topRight;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
            Source = source;
            Tint = tint;
            Blend = blend;
        }
    }
}