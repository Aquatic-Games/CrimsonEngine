using Crimson.Math;
using Silk.NET.OpenGL;

namespace Crimson.Graphics.RHI.OpenGL;

public class GLGraphicsDevice : GraphicsDevice
{
    private readonly GLContext _context;
    private readonly GL _gl;
    
    public override Backend Backend => Backend.OpenGL;

    public GLGraphicsDevice(GLContext context)
    {
        _context = context;
        _gl = GL.GetApi(_context.GetProcAddressFunc);
    }
    
    public override void BeginRendering(Color color)
    {
        _gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        _gl.ClearColor(color.R, color.G, color.B, color.A);
        _gl.Clear(ClearBufferMask.ColorBufferBit);
    }
    
    public override void EndRendering() { }
    
    public override void Present()
    {
        _context.PresentFunc(1);
    }
    
    public override void Dispose()
    {
        _gl.Dispose();
    }
}