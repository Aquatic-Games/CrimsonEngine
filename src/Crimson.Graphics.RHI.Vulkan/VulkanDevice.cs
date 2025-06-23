using Crimson.Core;
using SDL3;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;

namespace Crimson.Graphics.RHI.Vulkan;

public sealed unsafe class VulkanDevice : GraphicsDevice
{
    private readonly Vk _vk;
    private readonly Instance _instance;
    
    public VulkanDevice(string appName)
    {
        _vk = Vk.GetApi();
        
        nint pAppName = SilkMarshal.StringToPtr(appName);
        nint pEngineName = SilkMarshal.StringToPtr("Crimson");

        ApplicationInfo appInfo = new()
        {
            SType = StructureType.ApplicationInfo,
            PApplicationName = (byte*) pAppName,
            ApplicationVersion = Vk.MakeVersion(1, 0),
            PEngineName = (byte*) pEngineName,
            EngineVersion = Vk.MakeVersion(1, 0),
            ApiVersion = Vk.Version13
        };

        string[] instanceExtensions = SDL.VulkanGetInstanceExtensions(out uint extCount)!;
        nint pInstanceExtensions = SilkMarshal.StringArrayToPtr(instanceExtensions);

        InstanceCreateInfo instanceInfo = new()
        {
            SType = StructureType.InstanceCreateInfo,
            PApplicationInfo = &appInfo,
            
            EnabledExtensionCount = extCount,
            PpEnabledExtensionNames = (byte**) pInstanceExtensions
        };
        
        Logger.Trace("Creating instance.");
        _vk.CreateInstance(&instanceInfo, null, out _instance).Check("Create instance");

        SilkMarshal.Free(pInstanceExtensions);
        SilkMarshal.Free(pEngineName);
        SilkMarshal.Free(pAppName);
    }
    
    public override void Dispose()
    {
        _vk.DestroyInstance(_instance, null);
        _vk.Dispose();
    }
}