namespace Crimson.Graphics.RHI;

[Flags]
public enum ShaderStage
{
    None = 0,
    
    Vertex = 1 << 0,
    
    Pixel = 1 << 1,
    
    VertexPixel = Vertex | Pixel
}