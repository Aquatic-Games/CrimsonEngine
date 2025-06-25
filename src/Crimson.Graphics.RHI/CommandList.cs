namespace Crimson.Graphics.RHI;

public abstract class CommandList : IDisposable
{
    public abstract void Begin();

    public abstract void End();

    public abstract void BeginRenderPass(in ReadOnlySpan<ColorAttachmentInfo> colorAttachments);

    public abstract void EndRenderPass();

    public abstract void SetGraphicsPipeline(Pipeline pipeline);

    public abstract void Draw(uint numVertices);
    
    public abstract void Dispose();
}