using System.Numerics;
using Crimson.Math;
using Crimson.Render.Renderers.Structs;
using Crimson.Render.Utils;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;

namespace Crimson.Render.Renderers;

internal class DeferredRenderer : IDisposable
{
    private readonly D3D11Target _depthTarget;
    
    private readonly D3D11Target _albedoTarget;

    private readonly ID3D11VertexShader _gbufferVtx;
    private readonly ID3D11PixelShader _gBufferPxl;
    private readonly ID3D11InputLayout _gBufferLayout;

    private readonly ID3D11DepthStencilState _depthState;

    private readonly ID3D11VertexShader _passVtx;
    private readonly ID3D11PixelShader _passPxl;
    private readonly ID3D11DepthStencilState _passDepthState;

    private readonly ID3D11Buffer _cameraBuffer;
    private readonly ID3D11Buffer _worldBuffer;

    private readonly List<WorldRenderable> _drawQueue;
    
    public DeferredRenderer(ID3D11Device device, Size<int> size)
    {
        _depthTarget = new D3D11Target(device, Format.D32_Float, size, false);
        
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

        _depthState = device.CreateDepthStencilState(DepthStencilDescription.Default);
        
        ShaderUtils.LoadGraphicsShader(device, "Deferred/DeferredPass", out _passVtx!, out _passPxl!, out _);

        _passDepthState = device.CreateDepthStencilState(new DepthStencilDescription()
        {
            DepthEnable = true,
            DepthWriteMask = DepthWriteMask.Zero,
            DepthFunc = ComparisonFunction.Less
        });

        _cameraBuffer = device.CreateBuffer(CameraMatrices.SizeInBytes, BindFlags.ConstantBuffer, ResourceUsage.Dynamic,
            CpuAccessFlags.Write);
        
        _worldBuffer = device.CreateBuffer(64, BindFlags.ConstantBuffer, ResourceUsage.Dynamic, CpuAccessFlags.Write);

        _drawQueue = [];
    }

    public void AddToQueue(Renderable renderable, Matrix4x4 worldMatrix)
    {
        _drawQueue.Add(new WorldRenderable(renderable, worldMatrix));
    }

    public void Render(ID3D11DeviceContext context, ID3D11RenderTargetView renderTarget, CameraMatrices camera)
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
        
        context.OMSetDepthStencilState(_depthState);
        
        foreach ((Renderable renderable, Matrix4x4 world) in _drawQueue)
        {
            // TODO: This is inefficient. Ideally have a large buffer with offsets.
            //       When GRABS is implemented use push constants.
            context.UpdateBuffer(_worldBuffer, world);
            
            context.PSSetShaderResource(0, renderable.Material.Albedo.ResourceView);
            
            context.IASetVertexBuffer(0, renderable.VertexBuffer, Vertex.SizeInBytes);
            context.IASetIndexBuffer(renderable.IndexBuffer, Format.R32_UInt, 0);
            
            context.DrawIndexed(renderable.NumIndices, 0, 0);
        }
        
        #endregion

        #region Lighting Pass

        context.OMSetRenderTargets(renderTarget, _depthTarget.DepthTarget);
        
        context.VSSetShader(_passVtx);
        context.PSSetShader(_passPxl);
        context.OMSetDepthStencilState(_passDepthState);
        
        context.PSSetShaderResource(0, _albedoTarget.ResourceView!);
        
        context.Draw(6, 0);

        #endregion
        
        // TODO: Multi camera support.
        _drawQueue.Clear();
    }
    
    public void Dispose()
    {
        _worldBuffer.Dispose();
        _cameraBuffer.Dispose();
        _passDepthState.Dispose();
        _passPxl.Dispose();
        _passVtx.Dispose();
        _depthState.Dispose();
        _gBufferLayout.Dispose();
        _gBufferPxl.Dispose();
        _gbufferVtx.Dispose();
        _albedoTarget.Dispose();
        _depthTarget.Dispose();
    }
}