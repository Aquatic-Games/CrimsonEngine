using Crimson.Core;
using SDL3;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace Crimson.Graphics.Vulkan;

public sealed unsafe class GraphicsDevice : IDisposable
{
    private readonly Vk _vk;
    private readonly Instance _instance;

    private readonly SurfaceKHR _surface;
    
    private readonly PhysicalDevice _physicalDevice;
    private readonly Device _device;

    private readonly uint _graphicsQueueIndex;
    private readonly uint _presentQueueIndex;

    private readonly Queue _graphicsQueue;
    private readonly Queue _presentQueue;

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

        uint? graphicsQueueIndex = null;
        uint? presentQueueIndex = null;
        
        for (uint i = 0; i < numQueueFamilies; i++)
        {
            if ((queueFamilyProperties[i].QueueFlags & QueueFlags.GraphicsBit) != 0)
                graphicsQueueIndex = i;

            if (SDL.VulkanGetPresentationSupport(_instance.Handle, _physicalDevice.Handle, i))
                presentQueueIndex = i;

            if (graphicsQueueIndex.HasValue && presentQueueIndex.HasValue)
                break;
        }

        if (!graphicsQueueIndex.HasValue || !presentQueueIndex.HasValue)
        {
            throw new Exception(
                $"GPU device does not support graphics and/or presentation! graphicsQueueIndex: {graphicsQueueIndex}, presentQueueIndex: {presentQueueIndex}");
        }

        _graphicsQueueIndex = graphicsQueueIndex.Value;
        _presentQueueIndex = presentQueueIndex.Value;

        HashSet<uint> uniqueQueueFamilies = [_graphicsQueueIndex, _presentQueueIndex];

        DeviceQueueCreateInfo* queueInfos = stackalloc DeviceQueueCreateInfo[uniqueQueueFamilies.Count];
        
        int queueFamilyIndex = 0;
        float queuePriority = 1.0f;
        foreach (uint family in uniqueQueueFamilies)
        {
            queueInfos[queueFamilyIndex] = new DeviceQueueCreateInfo
            {
                SType = StructureType.DeviceQueueCreateInfo,
                QueueCount = 1,
                QueueFamilyIndex = family,
                PQueuePriorities = &queuePriority
            };
        }

        string[] deviceExtensions = [KhrSwapchain.ExtensionName];
        nint pDeviceExtensions = SilkMarshal.StringArrayToPtr(deviceExtensions);
        
        Logger.Trace($"Device extensions: [{string.Join(", ", deviceExtensions)}]");

        PhysicalDeviceFeatures enabledFeatures = new() { };

        DeviceCreateInfo deviceInfo = new()
        {
            SType = StructureType.DeviceCreateInfo,

            QueueCreateInfoCount = (uint) uniqueQueueFamilies.Count,
            PQueueCreateInfos = queueInfos,
            
            EnabledExtensionCount = (uint) deviceExtensions.Length,
            PpEnabledExtensionNames = (byte**) pDeviceExtensions,
            
            PEnabledFeatures = &enabledFeatures
        };
        
        Logger.Trace("Creating device.");
        _vk.CreateDevice(_physicalDevice, &deviceInfo, null, out _device).Check("Create device");

        SilkMarshal.Free(pDeviceExtensions);
    }
    
    public void Dispose()
    {
        _vk.DestroyDevice(_device, null);
        SDL.VulkanDestroySurface(_instance.Handle, (IntPtr) _surface.Handle, IntPtr.Zero);
        _vk.DestroyInstance(_instance, null);
        _vk.Dispose();
    }
}