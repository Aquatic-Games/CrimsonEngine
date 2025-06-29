using Vortice.Direct3D;
using Vortice.Direct3D11;

namespace Crimson.Graphics.RHI.D3D11;

internal sealed class D3D11Pipeline : Pipeline
{
    public readonly ID3D11VertexShader VertexShader;
    public readonly ID3D11PixelShader PixelShader;

    public readonly PrimitiveTopology PrimitiveTopology;
    
    public readonly ID3D11DepthStencilState DepthStencilState;
    public readonly ID3D11RasterizerState RasterizerState;
    
    public D3D11Pipeline(ID3D11Device device, in GraphicsPipelineInfo info)
    {
        D3D11ShaderModule vertexShader = (D3D11ShaderModule) info.VertexShader;
        VertexShader = device.CreateVertexShader(vertexShader.Data);
        
        D3D11ShaderModule pixelShader = (D3D11ShaderModule) info.PixelShader;
        PixelShader = device.CreatePixelShader(pixelShader.Data);

        PrimitiveTopology = PrimitiveTopology.TriangleList;
        
        DepthStencilState = device.CreateDepthStencilState(DepthStencilDescription.None);
        RasterizerState = device.CreateRasterizerState(RasterizerDescription.CullNone);
    }
    
    public override void Dispose()
    {
        RasterizerState.Dispose();
        DepthStencilState.Dispose();
        PixelShader.Dispose();
        VertexShader.Dispose();
    }
}