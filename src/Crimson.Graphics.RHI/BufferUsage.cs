namespace Crimson.Graphics.RHI;

[Flags]
public enum BufferUsage
{
    /// <summary>
    /// This buffer will not be used. It will use memory just to make you suffer.
    /// </summary>
    None = 0,
    
    VertexBuffer = 1 << 0,
    
    IndexBuffer = 1 << 1,
    
    ConstantBuffer = 1 << 2
}