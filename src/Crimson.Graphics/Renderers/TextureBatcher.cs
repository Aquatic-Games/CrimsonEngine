using System.Diagnostics;
using System.Numerics;
using Crimson.Graphics.Renderers.Structs;
using Crimson.Graphics.Utils;
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

    private readonly ID3D11Buffer _vertexBuffer;
    private readonly ID3D11Buffer _indexBuffer;
    private readonly ID3D11Buffer _cameraBuffer;

    private readonly ID3D11VertexShader _vertexShader;
    private readonly ID3D11PixelShader _pixelShader;
    private readonly ID3D11InputLayout _inputLayout;

    private readonly List<Draw> _drawQueue;
    
    public TextureBatcher(ID3D11Device device)
    {
        _vertices = new Vertex[MaxVertices];
        _indices = new uint[MaxIndices];

        _vertexBuffer = device.CreateBuffer(MaxVertices * Vertex.SizeInBytes, BindFlags.VertexBuffer,
            ResourceUsage.Dynamic, CpuAccessFlags.Write);

        _indexBuffer = device.CreateBuffer(MaxIndices * sizeof(uint), BindFlags.IndexBuffer, ResourceUsage.Dynamic,
            CpuAccessFlags.Write);

        _cameraBuffer = device.CreateBuffer(CameraMatrices.SizeInBytes, BindFlags.ConstantBuffer, ResourceUsage.Dynamic,
            CpuAccessFlags.Write);
        
        ShaderUtils.LoadGraphicsShader(device, "Texture", out _vertexShader!, out _pixelShader!, out byte[] bytecode);

        InputElementDescription[] elements =
        [
            new InputElementDescription("POSITION", 0, Format.R32G32_Float, 0, 0),
            new InputElementDescription("TEXCOORD", 0, Format.R32G32_Float, 8, 0),
            new InputElementDescription("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
        ];

        _inputLayout = device.CreateInputLayout(elements, bytecode!);

        _drawQueue = [];
    }

    public void AddToDrawQueue(in Draw draw)
    {
        _drawQueue.Add(draw);
    }

    public void DispatchDrawQueue(ID3D11DeviceContext context, Matrix4x4 projection, Matrix4x4 transform)
    {
        MappedSubresource cameraRes = context.Map(_cameraBuffer, MapMode.WriteDiscard);
        CameraMatrices matrices = new CameraMatrices(projection, transform);
        UnsafeUtilities.Write(cameraRes.DataPointer, ref matrices);
        context.Unmap(_cameraBuffer);
        
        context.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);
        context.IASetInputLayout(_inputLayout);
        
        context.VSSetShader(_vertexShader);
        context.PSSetShader(_pixelShader);
        
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