using Crimson.Core;
using SDL3;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;

namespace Crimson.Graphics.Vulkan;

public sealed unsafe class GraphicsDevice : IDisposable
{
    private readonly Vk _vk;
    private readonly Instance _instance;

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
        
        Logger.Trace($"Instance Extensions: [{string.Join(", ", instanceExtensions)}]");

        nint pInstanceExtensions = SilkMarshal.StringArrayToPtr(instanceExtensions);

        InstanceCreateInfo instanceInfo = new()
        {
            SType = StructureType.InstanceCreateInfo,
            PApplicationInfo = &appInfo,
            
            EnabledExtensionCount = (uint) instanceExtensions.Length,
            PpEnabledExtensionNames = (byte**) pInstanceExtensions
        };
        
        Logger.Trace("Creating instance.");
        _vk.CreateInstance(&instanceInfo, null, out _instance).Check("Create instance");

        SilkMarshal.Free(pInstanceExtensions);
        SilkMarshal.FreeString(pEngineName);
        SilkMarshal.FreeString(pAppName);
    }
    
    public void Dispose()
    {
        _vk.DestroyInstance(_instance, null);
        _vk.Dispose();
    }
}