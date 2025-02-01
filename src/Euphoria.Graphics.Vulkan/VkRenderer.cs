using Euphoria.Graphics.Vulkan.RenderAbstraction;

namespace Euphoria.Graphics.Vulkan;

public class VkRenderer : Renderer
{
    internal readonly VkDevice Device;
    
    public VkRenderer(in RendererInfo info)
    {
        Device = new VkDevice(info.Debug);
    }
    
    public override void Present()
    {
        throw new NotImplementedException();
    }
    
    public override void Dispose()
    {
        Device.Dispose();
    }
}