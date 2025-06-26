namespace Crimson.Graphics.RHI;

public abstract class Device : IDisposable
{
    public abstract Backend Backend { get; }
    
    public abstract Format SwapchainFormat { get; }

    public abstract CommandList CreateCommandList();

    public abstract ShaderModule CreateShaderModule(ShaderStage stage, byte[] compiled, string entryPoint);

    public abstract Pipeline CreateGraphicsPipeline(in GraphicsPipelineInfo info);

    public abstract Buffer CreateBuffer<T>(BufferUsage usage, in ReadOnlySpan<T> data) where T : unmanaged;

    public abstract void ExecuteCommandList(CommandList cl);

    public abstract Texture GetNextSwapchainTexture();

    public abstract void Present();
    
    public abstract void Dispose();
}