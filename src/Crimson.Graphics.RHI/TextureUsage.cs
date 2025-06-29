namespace Crimson.Graphics.RHI;

[Flags]
public enum TextureUsage
{
    None = 0,
    
    ShaderResource = 1 << 0,
    
    ColorAttachment = 1 << 1,
    
    DepthStencil = 1 << 2,
    
    GenerateMips = 1 << 3
}