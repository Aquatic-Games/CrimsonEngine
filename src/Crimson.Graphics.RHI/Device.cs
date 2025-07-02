using Crimson.Math;

namespace Crimson.Graphics.RHI;

public abstract class Device : IDisposable
{
    public abstract Backend Backend { get; }
    
    public abstract Format SwapchainFormat { get; }

    public abstract CommandList CreateCommandList();

    public abstract ShaderModule CreateShaderModule(byte[] compiled, string entryPoint);

    public abstract Pipeline CreateGraphicsPipeline(in GraphicsPipelineInfo info);

    public abstract Buffer CreateBuffer(BufferUsage usage, uint sizeInBytes);

    public abstract Texture CreateTexture(in TextureInfo info);

    public abstract void ExecuteCommandList(CommandList cl);

    public abstract nint MapBuffer(Buffer buffer);

    public abstract void UnmapBuffer(Buffer buffer);

    public abstract Texture GetNextSwapchainTexture();

    public abstract void Present();

    public abstract void Resize(Size<uint> newSize);
    
    public abstract void Dispose();
}