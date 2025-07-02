using Vortice.Direct3D;
using Vortice.Direct3D11;

namespace Crimson.Graphics.RHI.D3D11;

internal sealed class D3D11Pipeline : Pipeline
{
    public readonly ID3D11VertexShader VertexShader;
    public readonly ID3D11PixelShader PixelShader;

    public readonly ID3D11InputLayout? InputLayout;

    public readonly PrimitiveTopology PrimitiveTopology;
    
    public readonly ID3D11DepthStencilState DepthStencilState;
    public readonly ID3D11RasterizerState RasterizerState;
    
    public D3D11Pipeline(ID3D11Device device, in GraphicsPipelineInfo info)
    {
        D3D11ShaderModule vertexShader = (D3D11ShaderModule) info.VertexShader;
        VertexShader = device.CreateVertexShader(vertexShader.Data);
        
        D3D11ShaderModule pixelShader = (D3D11ShaderModule) info.PixelShader;
        PixelShader = device.CreatePixelShader(pixelShader.Data);

        if (info.InputLayout.Length > 0)
        {
            Vortice.Direct3D11.InputElementDescription[] inputElements =
                new Vortice.Direct3D11.InputElementDescription[info.InputLayout.Length];

            for (int i = 0; i < info.InputLayout.Length; i++)
            {
                ref readonly InputElementDescription element = ref info.InputLayout[i];
                string semantic = element.Semantic.ToD3D();

                inputElements[i] = new Vortice.Direct3D11.InputElementDescription(semantic, 0, element.Format.ToD3D(),
                    element.Offset, element.Slot);
            }

            InputLayout = device.CreateInputLayout(inputElements, vertexShader.Data);
        }

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