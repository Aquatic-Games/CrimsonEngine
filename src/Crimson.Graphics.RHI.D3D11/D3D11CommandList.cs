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
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }
    
    public override void SetVertexBuffer(uint slot, Buffer buffer, uint offset = 0)
    {
        throw new NotImplementedException();
    }
    
    public override void SetIndexBuffer(Buffer buffer, Format format, uint offset = 0)
    {
        throw new NotImplementedException();
    }
    
    public override void Draw(uint numVertices)
    {
        throw new NotImplementedException();
    }
    
    public override void DrawIndexed(uint numIndices)
    {
        throw new NotImplementedException();
    }
    
    public override void Dispose()
    {
        CommandList?.Dispose();
        _context.Dispose();
    }
}