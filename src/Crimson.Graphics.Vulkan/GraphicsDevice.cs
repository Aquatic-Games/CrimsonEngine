using Crimson.Core;
using SDL3;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;

namespace Crimson.Graphics.Vulkan;

public sealed unsafe class GraphicsDevice : IDisposable
{
    private readonly Vk _vk;
    private readonly Instance _instance;

    private readonly SurfaceKHR _surface;
    
    private readonly PhysicalDevice _physicalDevice;

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

        Logger.Trace("Creating surface.");
        if (!SDL.VulkanCreateSurface(sdlWindow, _instance.Handle, IntPtr.Zero, out IntPtr surface))
            throw new Exception($"Failed to create vulkan surface: {SDL.GetError()}");
        _surface = new SurfaceKHR((ulong) surface);

        Logger.Trace("Selecting physical device.");
        uint numPhysicalDevices;
        _vk.EnumeratePhysicalDevices(_instance, &numPhysicalDevices, null);
        PhysicalDevice* physicalDevices = stackalloc PhysicalDevice[(int) numPhysicalDevices];
        _vk.EnumeratePhysicalDevices(_instance, &numPhysicalDevices, physicalDevices);

        // TODO: Better selection of physical device.
        _physicalDevice = physicalDevices[0];

        PhysicalDeviceProperties physicalDeviceProperties;
        _vk.GetPhysicalDeviceProperties(_physicalDevice, &physicalDeviceProperties);
        Version32 supportedVersion = (Version32) physicalDeviceProperties.ApiVersion;
        
        Logger.Info($"Using device: {new string((sbyte*) physicalDeviceProperties.DeviceName)}");
        Logger.Info($"    Type: {physicalDeviceProperties.DeviceType}");
        Logger.Info($"    Supported version: {supportedVersion.Major}.{supportedVersion.Minor}.{supportedVersion.Patch}");
        
        uint numQueueFamilies;
        _vk.GetPhysicalDeviceQueueFamilyProperties(_physicalDevice, &numQueueFamilies, null);
        QueueFamilyProperties* queueFamilyProperties = stackalloc QueueFamilyProperties[(int) numQueueFamilies];
        _vk.GetPhysicalDeviceQueueFamilyProperties(_physicalDevice, &numQueueFamilies, queueFamilyProperties);
        
        _vk
    }
    
    public void Dispose()
    {
        SDL.VulkanDestroySurface(_instance.Handle, (IntPtr) _surface.Handle, IntPtr.Zero);
        _vk.DestroyInstance(_instance, null);
        _vk.Dispose();
    }
}