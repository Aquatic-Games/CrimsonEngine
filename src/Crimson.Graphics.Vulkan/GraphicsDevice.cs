using SDL3;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;

namespace Crimson.Graphics.Vulkan;

public sealed unsafe class GraphicsDevice : IDisposable
{
    private readonly Vk _vk;

    public GraphicsDevice(string appName, IntPtr sdlWindow)
    {
        _vk = Vk.GetApi();

        nint pAppName = SilkMarshal.StringToPtr(appName);
        nint pEngineName = SilkMarshal.StringToPtr("Crimson");

        ApplicationInfo appInfo = new()
        {
            SType = StructureType.ApplicationInfo,
            PApplicationName = (byte*) pAppName,
            PEngineName = (byte*) pEngineName,
            ApiVersion = Vk.Version10
        };

        string[]? instanceExtensions = SDL.VulkanGetInstanceExtensions(out _);
        if (instanceExtensions == null)
            throw new Exception("instanceExtensions was null. Is Vulkan supported?");

        InstanceCreateInfo instanceInfo = new()
        {
            SType = StructureType.InstanceCreateInfo,
            PApplicationInfo = &appInfo
        };
        
        SilkMarshal.FreeString(pEngineName);
        SilkMarshal.FreeString(pAppName);
    }
    
    public void Dispose()
    {
        _vk.Dispose();
    }
}