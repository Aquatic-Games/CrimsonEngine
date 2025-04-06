using System.Numerics;
using Euphoria.Math;
using Euphoria.Render.Renderers.Structs;
using Euphoria.Render.Utils;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;

namespace Euphoria.Render.Renderers;

internal class DeferredRenderer : IDisposable
{
    private readonly D3D11Target _depthTarget;
    
    private readonly D3D11Target _albedoTarget;

    private readonly ID3D11VertexShader _gbufferVtx;
    private readonly ID3D11PixelShader _gBufferPxl;
    private readonly ID3D11InputLayout _gBufferLayout;

    private readonly ID3D11Buffer _cameraBuffer;
    private readonly ID3D11Buffer _worldBuffer;

    private readonly List<WorldRenderable> _drawQueue;
    
    public DeferredRenderer(ID3D11Device device, Size<int> size)
    {
        _depthTarget = new D3D11Target(device, Format.D32_Float, size);
        
        _albedoTarget = new D3D11Target(device, Format.R32G32B32A32_Float, size);

        ShaderUtils.LoadGraphicsShader(device, "Deferred/GBuffer", out _gbufferVtx!, out _gBufferPxl!,
            out byte[] vtxCode);

        InputElementDescription[] gBufferElements =
        [
            new InputElementDescription("POSITION", 0, Format.R32G32B32_Float, 0, 0),
            new InputElementDescription("TEXCOORD", 0, Format.R32G32_Float, 12, 0),
            new InputElementDescription("COLOR", 0, Format.R32G32B32A32_Float, 20, 0),
            new InputElementDescription("NORMAL", 0, Format.R32G32B32_Float, 36, 0)
        ];

        _gBufferLayout = device.CreateInputLayout(gBufferElements, vtxCode!);

        _cameraBuffer = device.CreateBuffer(CameraMatrices.SizeInBytes, BindFlags.ConstantBuffer, ResourceUsage.Dynamic,
            CpuAccessFlags.Write);
        
        _worldBuffer = device.CreateBuffer(64, BindFlags.ConstantBuffer, ResourceUsage.Dynamic, CpuAccessFlags.Write);

        _drawQueue = [];
    }

    public void AddToQueue(Renderable renderable, Matrix4x4 worldMatrix)
    {
        _drawQueue.Add(new WorldRenderable(renderable, worldMatrix));
    }

    public void Render(ID3D11DeviceContext context, CameraMatrices camera)
    {
        context.UpdateBuffer(_cameraBuffer, camera);
        
        #region GBuffer Pass
        
        Span<ID3D11RenderTargetView> targets = [_albedoTarget.RenderTarget!];
        context.OMSetRenderTargets(targets, _depthTarget.DepthTarget!);
        
        context.ClearRenderTargetView(_albedoTarget.RenderTarget, new Color4(0.0f, 0.0f, 0.0f, 0.0f));
        context.ClearDepthStencilView(_depthTarget.DepthTarget, DepthStencilClearFlags.Depth, 1, 0);

        context.VSSetConstantBuffer(0, _cameraBuffer);
        context.VSSetConstantBuffer(2, _worldBuffer);
            
        context.VSSetShader(_gbufferVtx);
        context.PSSetShader(_gBufferPxl);
            
        context.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);
        context.IASetInputLayout(_gBufferLayout);
        
        foreach ((Renderable renderable, Matrix4x4 world) in _drawQueue)
        {
            // TODO: This is inefficient. Ideally have a large buffer with offsets.
            //       When GRABS is implemented use push constants.
            context.UpdateBuffer(_worldBuffer, world);
            
            context.IASetVertexBuffer(0, renderable.VertexBuffer, Vertex.SizeInBytes);
            context.IASetIndexBuffer(renderable.IndexBuffer, Format.R32_UInt, 0);
            
            context.DrawIndexed(renderable.NumIndices, 0, 0);
        }
        
        #endregion
        
        // TODO: Multi camera support.
        _drawQueue.Clear();
    }
    
    public void Dispose()
    {
        _worldBuffer.Dispose();
        _cameraBuffer.Dispose();
        _gBufferLayout.Dispose();
        _gBufferPxl.Dispose();
        _gbufferVtx.Dispose();
        _albedoTarget.Dispose();
        _depthTarget.Dispose();
    }
}