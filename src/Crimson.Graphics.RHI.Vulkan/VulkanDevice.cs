global using VkDevice = Silk.NET.Vulkan.Device;
using System.Diagnostics;
using Crimson.Core;
using SDL3;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace Crimson.Graphics.RHI.Vulkan;

public sealed unsafe class VulkanDevice : Device
{
    private readonly Vk _vk;
    private readonly Instance _instance;

    private readonly KhrSurface _surfaceExt;
    private readonly SurfaceKHR _surface;

    private readonly Queues _queues;
    
    private readonly PhysicalDevice _physicalDevice;
    private readonly VkDevice _device;
    
    public override Backend Backend => Backend.Vulkan;
    
    public VulkanDevice(string appName, IntPtr sdlWindow)
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
        
        Logger.Trace("Creating window surface.");
        if (!_vk.TryGetInstanceExtension(_instance, out _surfaceExt))
            throw new Exception("Failed to get surface extension.");

        if (!SDL.VulkanCreateSurface(sdlWindow, _instance.Handle, IntPtr.Zero, out IntPtr surfacePtr))
            throw new Exception($"Failed to create surface: {SDL.GetError()}");
        _surface = new SurfaceKHR((ulong) surfacePtr);
        
        Logger.Trace("Picking physical device.");

        uint numPhysicalDevices;
        _vk.EnumeratePhysicalDevices(_instance, &numPhysicalDevices, null).Check("Enumerate physical devices");
        PhysicalDevice* physicalDevices = stackalloc PhysicalDevice[(int) numPhysicalDevices];
        _vk.EnumeratePhysicalDevices(_instance, &numPhysicalDevices, physicalDevices).Check("Enumerate physical devices");

        // TODO: Actually check the correct device instead of assuming the first device is supported.
        {
            PhysicalDevice physDevice = physicalDevices[0];
            _physicalDevice = physDevice;
        }

        PhysicalDeviceProperties physProps;
        _vk.GetPhysicalDeviceProperties(_physicalDevice, &physProps);
        
        Logger.Info($"Using adapter '{new string((sbyte*) physProps.DeviceName)}'");

        uint numQueueFamilies;
        _vk.GetPhysicalDeviceQueueFamilyProperties(_physicalDevice, &numQueueFamilies, null);
        QueueFamilyProperties* queueProperties = stackalloc QueueFamilyProperties[(int) numQueueFamilies];
        _vk.GetPhysicalDeviceQueueFamilyProperties(_physicalDevice, &numQueueFamilies, queueProperties);
        
        uint? graphicsQueue = null;
        uint? presentQueue = null;

        for (uint i = 0; i < numQueueFamilies; i++)
        {
            if (queueProperties[i].QueueFlags.HasFlag(QueueFlags.GraphicsBit))
                graphicsQueue = i;

            _surfaceExt.GetPhysicalDeviceSurfaceSupport(_physicalDevice, i, _surface, out Bool32 supported);
            if (supported)
                presentQueue = i;

            if (graphicsQueue.HasValue && presentQueue.HasValue)
                break;
        }

        if (!graphicsQueue.HasValue || !presentQueue.HasValue)
            throw new Exception("Device not supported. One or more queues are missing.");

        _queues.GraphicsIndex = graphicsQueue.Value;
        _queues.PresentIndex = presentQueue.Value;

        HashSet<uint> uniqueQueueFamilies = [_queues.GraphicsIndex, _queues.PresentIndex];
        DeviceQueueCreateInfo* queueInfos = stackalloc DeviceQueueCreateInfo[uniqueQueueFamilies.Count];

        float queuePriority = 1.0f;

        uint currentQueue = 0;
        foreach (uint family in uniqueQueueFamilies)
        {
            queueInfos[currentQueue++] = new DeviceQueueCreateInfo()
            {
                SType = StructureType.DeviceQueueCreateInfo,
                PQueuePriorities = &queuePriority,
                QueueCount = 1,
                QueueFamilyIndex = family
            };
        }
        
        // Failure condition, means that it's written into memory it shouldn't have.
        Debug.Assert(currentQueue == uniqueQueueFamilies.Count);
        
        PhysicalDeviceFeatures deviceFeatures = new();

        DeviceCreateInfo deviceInfo = new()
        {
            SType = StructureType.DeviceCreateInfo,
            PEnabledFeatures = &deviceFeatures,
            
            QueueCreateInfoCount = (uint) uniqueQueueFamilies.Count,
            PQueueCreateInfos = queueInfos
        };
        
        Logger.Trace("Creating logical device.");
        _vk.CreateDevice(_physicalDevice, &deviceInfo, null, out _device).Check("Create device");
        
        Logger.Trace("Getting device queues.");
        _vk.GetDeviceQueue(_device, _queues.GraphicsIndex, 0, out _queues.Graphics);
        _vk.GetDeviceQueue(_device, _queues.PresentIndex, 0, out _queues.Present);
    }

    public override void Dispose()
    {
        _vk.DestroyDevice(_device, null);
        _surfaceExt.DestroySurface(_instance, _surface, null);
        _surfaceExt.Dispose();
        _vk.DestroyInstance(_instance, null);
        _vk.Dispose();
    }
}