namespace Crimson.Graphics.RHI;

public abstract class GraphicsDevice : IDisposable
{
    public bool IsDisposed { get; protected set; }
    
    public abstract Backend Backend { get; }

    public abstract void Present();
    
    public abstract void Dispose();
}