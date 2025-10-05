namespace Crimson.Graphics.RHI.OpenGL;

public class GLGraphicsDevice : GraphicsDevice
{
    private readonly GLContext _context;
    
    public override Backend Backend => Backend.OpenGL;

    public GLGraphicsDevice(GLContext context)
    {
        _context = context;
    }
    
    public override void Present()
    {
        _context.PresentFunc(1);
    }
    
    public override void Dispose()
    {
        throw new NotImplementedException();
    }
}