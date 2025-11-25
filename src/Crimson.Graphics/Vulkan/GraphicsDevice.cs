using Silk.NET.Vulkan;

namespace Crimson.Graphics.Vulkan;

public sealed class GraphicsDevice : IDisposable
{
    private readonly Vk _vk;

    public GraphicsDevice(string appName)
    {
        _vk = Vk.GetApi();
        
        ApplicationInfo appInfo = new()
        {
            SType = StructureType.ApplicationInfo,
            PApplicationName = 
        }
    }
    
    public void Dispose()
    {
        
    }
}