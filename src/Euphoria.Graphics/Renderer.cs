namespace Euphoria.Graphics;

public abstract class Renderer : IDisposable
{
    public abstract void Present();
    
    public abstract void Dispose();
}