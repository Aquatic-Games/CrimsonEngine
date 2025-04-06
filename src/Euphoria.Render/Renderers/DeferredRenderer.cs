using System.Numerics;
using Euphoria.Math;
using Euphoria.Render.Renderers.Structs;
using Euphoria.Render.Utils;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace Euphoria.Render.Renderers;

internal class DeferredRenderer : IDisposable
{
    private readonly D3D11Target _albedoTarget;

    private readonly ID3D11VertexShader _gbufferVtx;
    private readonly ID3D11PixelShader _gBufferPxl;
    private readonly ID3D11InputLayout _gBufferLayout;

    private readonly List<WorldRenderable> _drawQueue;
    
    public DeferredRenderer(ID3D11Device device, Size<int> size)
    {
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

        _drawQueue = [];
    }

    public void AddToQueue(Renderable renderable, Matrix4x4 worldMatrix)
    {
        _drawQueue.Add(new WorldRenderable(renderable, worldMatrix));
    }

    public void Render(ID3D11DeviceContext context, CameraMatrices camera)
    {
        
    }
    
    public void Dispose()
    {
        _gBufferLayout.Dispose();
        _gBufferPxl.Dispose();
        _gbufferVtx.Dispose();
        _albedoTarget.Dispose();
    }
}