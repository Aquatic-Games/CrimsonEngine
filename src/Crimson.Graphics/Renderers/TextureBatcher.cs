using System.Diagnostics;
using System.Numerics;
using Crimson.Graphics.Renderers.Structs;
using Crimson.Graphics.Utils;
using Crimson.Math;
using SDL3;
using Vortice;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace Crimson.Graphics.Renderers;

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

    private readonly IntPtr _vertexBuffer;
    private readonly IntPtr _indexBuffer;

    private readonly IntPtr _pipeline;

    private readonly List<Draw> _drawQueue;
    
    public unsafe TextureBatcher(IntPtr device, SDL.GPUTextureFormat format)
    {
        _vertices = new Vertex[MaxVertices];
        _indices = new uint[MaxIndices];

        _vertexBuffer = SdlUtils.CreateBuffer(device, SDL.GPUBufferUsageFlags.Vertex, MaxVertices * Vertex.SizeInBytes);
        _indexBuffer = SdlUtils.CreateBuffer(device, SDL.GPUBufferUsageFlags.Index, MaxIndices * sizeof(uint));

        IntPtr vertexShader =
            ShaderUtils.LoadGraphicsShader(device, SDL.GPUShaderStage.Vertex, "Texture", "VSMain", 1, 0);
        IntPtr pixelShader =
            ShaderUtils.LoadGraphicsShader(device, SDL.GPUShaderStage.Fragment, "Texture", "PSMain", 0, 1);

        SDL.GPUColorTargetDescription targetDesc = new()
        {
            Format = format,
        };
        
        SDL.GPUGraphicsPipelineCreateInfo pipelineInfo = new()
        {
            VertexShader = vertexShader,
            FragmentShader = pixelShader,
            TargetInfo = { NumColorTargets = 1, ColorTargetDescriptions = new IntPtr(&targetDesc) },
            PrimitiveType = SDL.GPUPrimitiveType.TriangleList,
            
        }

        _drawQueue = [];
    }

    public void AddToDrawQueue(in Draw draw)
    {
        _drawQueue.Add(draw);
    }

    public void DispatchDrawQueue(IntPtr cb, in SDL.GPUColorTargetInfo passTarget, Matrix4x4 projection, Matrix4x4 transform)
    {
        MappedSubresource cameraRes = context.Map(_cameraBuffer, MapMode.WriteDiscard);
        CameraMatrices matrices = new CameraMatrices(projection, transform);
        UnsafeUtilities.Write(cameraRes.DataPointer, ref matrices);
        context.Unmap(_cameraBuffer);
        
        context.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);
        context.IASetInputLayout(_inputLayout);
        
        context.VSSetShader(_vertexShader);
        context.PSSetShader(_pixelShader);
        
        context.OMSetDepthStencilState(_depthState);
        context.RSSetState(_rasterizerState);
        context.OMSetBlendState(_blendState);
        
        context.IASetVertexBuffer(0, _vertexBuffer, Vertex.SizeInBytes);
        context.IASetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
        
        context.VSSetConstantBuffer(0, _cameraBuffer);

        uint numDraws = 0;
        Texture? texture = null;

        foreach (Draw draw in _drawQueue)
        {
            if (draw.Texture != texture || numDraws >= MaxBatches)
            {
                Flush(context, numDraws, texture);
                numDraws = 0;
            }

            texture = draw.Texture;

            uint vOffset = numDraws * NumVertices;
            uint iOffset = numDraws * NumIndices;

            _vertices[vOffset + 0] = new Vertex(draw.TopLeft, new Vector2(0, 0), draw.Tint);
            _vertices[vOffset + 1] = new Vertex(draw.TopRight, new Vector2(1, 0), draw.Tint);
            _vertices[vOffset + 2] = new Vertex(draw.BottomRight, new Vector2(1, 1), draw.Tint);
            _vertices[vOffset + 3] = new Vertex(draw.BottomLeft, new Vector2(0, 1), draw.Tint);

            _indices[iOffset + 0] = 0 + vOffset;
            _indices[iOffset + 1] = 1 + vOffset;
            _indices[iOffset + 2] = 3 + vOffset;
            _indices[iOffset + 3] = 1 + vOffset;
            _indices[iOffset + 4] = 2 + vOffset;
            _indices[iOffset + 5] = 3 + vOffset;

            numDraws++;
        }
        
        Flush(context, numDraws, texture);
        _drawQueue.Clear();
    }

    private void Flush(ID3D11DeviceContext context, uint numDraws, Texture? texture)
    {
        // Don't bother drawing unless there's stuff to draw.
        if (numDraws == 0)
            return;
        
        Debug.Assert(texture != null);

        MappedSubresource vMap = context.Map(_vertexBuffer, MapMode.WriteDiscard);
        UnsafeUtilities.Write(vMap.DataPointer, _vertices, 0, (int) (numDraws * NumVertices * Vertex.SizeInBytes));
        context.Unmap(_vertexBuffer);

        MappedSubresource iMap = context.Map(_indexBuffer, MapMode.WriteDiscard);
        UnsafeUtilities.Write(iMap.DataPointer, _indices, 0, (int) (numDraws * NumIndices * sizeof(uint)));
        context.Unmap(_indexBuffer);
        
        context.PSSetShaderResource(0, texture.ResourceView);
        
        context.DrawIndexed(numDraws * NumIndices, 0, 0);
    }
    
    public void Dispose()
    {
        _inputLayout.Dispose();
        _pixelShader.Dispose();
        _vertexShader.Dispose();
        
        _cameraBuffer.Dispose();
        _indexBuffer.Dispose();
        _vertexBuffer.Dispose();
    }

    private readonly struct Vertex
    {
        public const uint SizeInBytes = 32;
        
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

    public readonly struct Draw
    {
        public readonly Texture Texture;
        public readonly Vector2 TopLeft;
        public readonly Vector2 TopRight;
        public readonly Vector2 BottomLeft;
        public readonly Vector2 BottomRight;
        public readonly Color Tint;

        public Draw(Texture texture, Vector2 topLeft, Vector2 topRight, Vector2 bottomLeft, Vector2 bottomRight, Color tint)
        {
            Texture = texture;
            TopLeft = topLeft;
            TopRight = topRight;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
            Tint = tint;
        }
    }
}