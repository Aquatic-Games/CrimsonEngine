namespace Crimson.Graphics.RHI;

public abstract class Device : IDisposable
{
    public abstract Backend Backend { get; }

    public abstract CommandList CreateCommandList();

    public abstract Texture GetNextSwapchainTexture();

    public abstract void Present();
    
    public abstract void Dispose();
}