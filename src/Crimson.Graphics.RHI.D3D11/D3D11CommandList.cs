using System.Diagnostics;
using Vortice.Direct3D11;
using Vortice.Mathematics;
using Color = Crimson.Math.Color;

namespace Crimson.Graphics.RHI.D3D11;

internal sealed class D3D11CommandList : CommandList
{
    private readonly ID3D11DeviceContext _context;

    public ID3D11CommandList? CommandList;

    public D3D11CommandList(ID3D11Device device)
    {
        _context = device.CreateDeferredContext();
    }
    
    public override void Begin()
    {
        CommandList?.Dispose();
        CommandList = null;
    }
    
    public override void End()
    {
        CommandList = _context.FinishCommandList(false);
    }
    
    public override void CopyBufferToBuffer(Buffer source, uint srcOffset, Buffer dest, uint destOffset, uint copySize = 0)
    {
        D3D11Buffer d3dSrc = (D3D11Buffer) source;
        D3D11Buffer d3dDest = (D3D11Buffer) dest;

        _context.CopySubresourceRegion(d3dDest.Buffer, 0, destOffset, 0, 0, d3dSrc.Buffer, 0,
            new Box((int) srcOffset, 0, 0, (int) (srcOffset + (copySize == 0 ? d3dSrc.BufferSize : copySize)), 1, 1));
    }
    
    public override void BeginRenderPass(in ReadOnlySpan<ColorAttachmentInfo> colorAttachments)
    {
        ID3D11RenderTargetView[] renderTargets = new ID3D11RenderTargetView[colorAttachments.Length];

        for (int i = 0; i < colorAttachments.Length; i++)
        {
            D3D11Texture texture = (D3D11Texture) colorAttachments[i].Texture;
            Debug.Assert(texture.RenderTarget != null);
            renderTargets[i] = texture.RenderTarget;
        }
        
        _context.OMSetRenderTargets(renderTargets);

        // According to DXVK docs, it's more efficient to set, then clear. I'll have to work out if that's true or not
        // as I'd much rather clear in the first loop as well.
        for (int i = 0; i < colorAttachments.Length; i++)
        {
            ref readonly ColorAttachmentInfo attachment = ref colorAttachments[i];
            D3D11Texture texture = (D3D11Texture) attachment.Texture;

            if (attachment.LoadOp != LoadOp.Clear)
                continue;
            
            Color color = attachment.ClearColor;
            _context.ClearRenderTargetView(texture.RenderTarget, new Color4(color.R, color.G, color.B, color.A));
        }

        Texture firstTexture = colorAttachments[0].Texture;
        _context.RSSetViewport(0, 0, firstTexture.Size.Width, firstTexture.Size.Height);
    }
    
    public override void EndRenderPass() { }
    
    public override void SetGraphicsPipeline(Pipeline pipeline)
    {
        D3D11Pipeline d3dPipeline = (D3D11Pipeline) pipeline;
        _context.VSSetShader(d3dPipeline.VertexShader);
        _context.PSSetShader(d3dPipeline.PixelShader);
        
        if (d3dPipeline.InputLayout != null)
            _context.IASetInputLayout(d3dPipeline.InputLayout);

        _context.IASetPrimitiveTopology(d3dPipeline.PrimitiveTopology);
        
        _context.OMSetDepthStencilState(d3dPipeline.DepthStencilState);
        _context.RSSetState(d3dPipeline.RasterizerState);
    }
    
    public override void SetVertexBuffer(uint slot, Buffer buffer, uint stride, uint offset = 0)
    {
        D3D11Buffer d3dBuffer = (D3D11Buffer) buffer;
        _context.IASetVertexBuffer(slot, d3dBuffer.Buffer, stride, offset);
    }
    
    public override void SetIndexBuffer(Buffer buffer, Format format, uint offset = 0)
    {
        D3D11Buffer d3dBuffer = (D3D11Buffer) buffer;
        _context.IASetIndexBuffer(d3dBuffer.Buffer, format.ToD3D(), offset);
    }
    
    public override void Draw(uint numVertices)
    {
        _context.Draw(numVertices, 0);
    }
    
    public override void DrawIndexed(uint numIndices)
    {
        _context.DrawIndexed(numIndices, 0, 0);
    }
    
    public override void Dispose()
    {
        CommandList?.Dispose();
        _context.Dispose();
    }
}