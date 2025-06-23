namespace Crimson.Graphics.RHI;

public abstract class Device : IDisposable
{
    public abstract Backend Backend { get; }
    
    public abstract void Dispose();
}