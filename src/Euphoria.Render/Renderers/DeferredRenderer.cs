using Euphoria.Math;
using Euphoria.Render.Utils;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace Euphoria.Render.Renderers;

internal class DeferredRenderer : IDisposable
{
    private readonly D3D11Target _albedoTarget;
    
    public DeferredRenderer(ID3D11Device device, Size<int> size)
    {
        _albedoTarget = new D3D11Target(device, Format.R32G32B32A32_Float, size);
    }
    
    public void Dispose()
    {
        _albedoTarget.Dispose();
    }
}