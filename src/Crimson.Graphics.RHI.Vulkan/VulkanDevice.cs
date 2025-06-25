global using VkDevice = Silk.NET.Vulkan.Device;
using System.Diagnostics;
using Crimson.Core;
using Crimson.Math;
using SDL3;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Vulkan.Extensions.KHR;
using Image = Silk.NET.Vulkan.Image;
using Semaphore = Silk.NET.Vulkan.Semaphore;

namespace Crimson.Graphics.RHI.Vulkan;

public sealed unsafe class VulkanDevice : Device
{
    private readonly Vk _vk;
    private readonly Instance _instance;

    private readonly ExtDebugUtils? _debugUtilsExt;
    private readonly DebugUtilsMessengerEXT _debugMessenger;

    private readonly KhrSurface _surfaceExt;
    private readonly SurfaceKHR _surface;

    private readonly Queues _queues;
    
    private readonly PhysicalDevice _physicalDevice;
    private readonly VkDevice _device;

    private readonly CommandPool _commandPool;

    private readonly KhrSwapchain _swapchainExt;
    private readonly Fence _swapchainFence;
    private SwapchainKHR _swapchain;
    private VulkanTexture[] _swapchainTextures;
    private uint _currentTexture;
    
    public override Backend Backend => Backend.Vulkan;

    public VulkanDevice(string appName, IntPtr sdlWindow, bool debug)
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

        string[] instanceExtensions = SDL.VulkanGetInstanceExtensions(out uint numExts)!;
        
        uint numLayers = 0;
        nint pLayers = 0;

        if (debug)
        {
            Logger.Debug("Debugging enabled. Inserting debug layers and exts.");
            Array.Resize(ref instanceExtensions, instanceExtensions.Length + 1);
            numExts++;
            instanceExtensions[^1] = ExtDebugUtils.ExtensionName;

            numLayers = 1;
            pLayers = SilkMarshal.StringArrayToPtr(["VK_LAYER_KHRONOS_validation"]);
        }
        
        nint pInstanceExtensions = SilkMarshal.StringArrayToPtr(instanceExtensions);

        InstanceCreateInfo instanceInfo = new()
        {
            SType = StructureType.InstanceCreateInfo,
            PApplicationInfo = &appInfo,
            
            EnabledExtensionCount = numExts,
            PpEnabledExtensionNames = (byte**) pInstanceExtensions,
            
            EnabledLayerCount = numLayers,
            PpEnabledLayerNames = (byte**) pLayers
        };
        
        Logger.Trace("Creating instance.");
        _vk.CreateInstance(&instanceInfo, null, out _instance).Check("Create instance");
        
        if (pLayers != 0)
            SilkMarshal.Free(pLayers);
        SilkMarshal.Free(pInstanceExtensions);
        SilkMarshal.Free(pEngineName);
        SilkMarshal.Free(pAppName);

        if (debug)
        {
            if (!_vk.TryGetInstanceExtension(_instance, out _debugUtilsExt))
                throw new Exception("Failed to get debug utils extension.");

            DebugUtilsMessengerCreateInfoEXT messengerInfo = new()
            {
                SType = StructureType.DebugUtilsMessengerCreateInfoExt,
                MessageType = DebugUtilsMessageTypeFlagsEXT.GeneralBitExt |
                              DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt |
                              DebugUtilsMessageTypeFlagsEXT.ValidationBitExt,
                MessageSeverity = DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt |
                                  DebugUtilsMessageSeverityFlagsEXT.InfoBitExt |
                                  DebugUtilsMessageSeverityFlagsEXT.WarningBitExt |
                                  DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt,
                PfnUserCallback = new PfnDebugUtilsMessengerCallbackEXT(DebugCallback)
            };

            Logger.Trace("Creating debug messenger.");
            _debugUtilsExt!.CreateDebugUtilsMessenger(_instance, &messengerInfo, null, out _debugMessenger)
                .Check("Create debug messenger");
        }
        
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

        numExts = 1;
        nint deviceExts = SilkMarshal.StringArrayToPtr([KhrSwapchain.ExtensionName]);
        
        PhysicalDeviceFeatures deviceFeatures = new();

        DeviceCreateInfo deviceInfo = new()
        {
            SType = StructureType.DeviceCreateInfo,
            PEnabledFeatures = &deviceFeatures,
            
            QueueCreateInfoCount = (uint) uniqueQueueFamilies.Count,
            PQueueCreateInfos = queueInfos,
            
            EnabledExtensionCount = numExts,
            PpEnabledExtensionNames = (byte**) deviceExts
        };

        // Enable dynamic rendering
        PhysicalDeviceDynamicRenderingFeatures dynamicRendering = new()
        {
            SType = StructureType.PhysicalDeviceDynamicRenderingFeatures,
            DynamicRendering = true
        };
        deviceInfo.PNext = &dynamicRendering;
        
        Logger.Trace("Creating logical device.");
        _vk.CreateDevice(_physicalDevice, &deviceInfo, null, out _device).Check("Create device");

        SilkMarshal.Free(deviceExts);
        
        Logger.Trace("Getting device queues.");
        _vk.GetDeviceQueue(_device, _queues.GraphicsIndex, 0, out _queues.Graphics);
        _vk.GetDeviceQueue(_device, _queues.PresentIndex, 0, out _queues.Present);

        CommandPoolCreateInfo poolInfo = new()
        {
            SType = StructureType.CommandPoolCreateInfo,
            QueueFamilyIndex = _queues.GraphicsIndex,
            Flags = CommandPoolCreateFlags.ResetCommandBufferBit
        };
        
        Logger.Trace("Creating command pool.");
        _vk.CreateCommandPool(_device, &poolInfo, null, out _commandPool).Check("Create command pool");

        if (!_vk.TryGetDeviceExtension(_instance, _device, out _swapchainExt))
            throw new Exception("Failed to get swapchain extension.");

        FenceCreateInfo swapchainFenceInfo = new()
        {
            SType = StructureType.FenceCreateInfo
        };
        
        Logger.Trace("Creating swapchain fence.");
        _vk.CreateFence(_device, &swapchainFenceInfo, null, out _swapchainFence).Check("Create swapchain fence");
        
        CreateSwapchain();
    }

    public override void Dispose()
    {
        _vk.DeviceWaitIdle(_device).Check("Wait for idle");
     
        foreach (VulkanTexture texture in _swapchainTextures)
            texture.Dispose();
        
        _swapchainExt.DestroySwapchain(_device, _swapchain, null);
        _swapchainExt.Dispose();
        
        _vk.DestroyFence(_device, _swapchainFence, null);
        
        _vk.DestroyCommandPool(_device, _commandPool, null);
        
        _vk.DestroyDevice(_device, null);
        
        _surfaceExt.DestroySurface(_instance, _surface, null);
        _surfaceExt.Dispose();

        if (_debugUtilsExt != null)
        {
            _debugUtilsExt.DestroyDebugUtilsMessenger(_instance, _debugMessenger, null);
            _debugUtilsExt.Dispose();
        }
        
        _vk.DestroyInstance(_instance, null);
        _vk.Dispose();
    }

    public override CommandList CreateCommandList()
    {
        return new VulkanCommandList(_vk, _device, _commandPool);
    }

    public override ShaderModule CreateShaderModule(ShaderStage stage, byte[] compiled, string entryPoint)
    {
        return new VulkanShaderModule(_vk, _device, stage, compiled, entryPoint);
    }

    public override void ExecuteCommandList(CommandList cl)
    {
        VulkanCommandList vulkanCl = (VulkanCommandList) cl;
        CommandBuffer buffer = vulkanCl.Buffer;

        SubmitInfo submitInfo = new()
        {
            SType = StructureType.SubmitInfo,
            CommandBufferCount = 1,
            PCommandBuffers = &buffer
        };

        // TODO: Must use semaphores etc for this. This is only temporary during development!
        // (2 years later....)
        _vk.QueueSubmit(_queues.Graphics, 1, &submitInfo, new Fence()).Check("Submit queue");
        _vk.QueueWaitIdle(_queues.Graphics).Check("Wait for queue idle");
    }

    public override Texture GetNextSwapchainTexture()
    {
        // TODO: Check for invalid swapchains and recreate.
        Result result = _swapchainExt.AcquireNextImage(_device, _swapchain, ulong.MaxValue, new Semaphore(),
            _swapchainFence, ref _currentTexture);
        
        // Recreate swapchain even when suboptimal, despite not being an error.
        if (result is Result.ErrorOutOfDateKhr or Result.ErrorSurfaceLostKhr)
            RecreateSwapchain();
        else if (result is not Result.SuboptimalKhr)
            result.Check("Acquire next swapchain image");

        _vk.WaitForFences(_device, 1, in _swapchainFence, true, ulong.MaxValue).Check("Wait for swapchain fence");
        _vk.ResetFences(_device, 1, in _swapchainFence).Check("Reset swapchain fence");

        return _swapchainTextures[_currentTexture];
    }
    
    public override void Present()
    {
        SwapchainKHR swapchain = _swapchain;
        uint currentImage = _currentTexture;

        PresentInfoKHR presentInfo = new()
        {
            SType = StructureType.PresentInfoKhr,
            SwapchainCount = 1,
            PSwapchains = &swapchain,
            PImageIndices = &currentImage
        };
        
        // TODO: Again, check for invalid swapchain and recreate.
        Result result = _swapchainExt.QueuePresent(_queues.Present, &presentInfo);
        
        // Recreate swapchain even when suboptimal, despite not being an error.
        if (result is Result.ErrorOutOfDateKhr or Result.ErrorSurfaceLostKhr)
            RecreateSwapchain();
        else if (result is not Result.SuboptimalKhr)
            result.Check("Present");
    }

    private void CreateSwapchain()
    {
        SurfaceCapabilitiesKHR capabilities;
        _surfaceExt.GetPhysicalDeviceSurfaceCapabilities(_physicalDevice, _surface, &capabilities)
            .Check("Get surface capabilities");

        Extent2D extent = capabilities.CurrentExtent;
        uint numImages = capabilities.MinImageCount;

        SwapchainCreateInfoKHR swapchainInfo = new()
        {
            SType = StructureType.SwapchainCreateInfoKhr,
            Surface = _surface,
            OldSwapchain = _swapchain,

            MinImageCount = numImages,
            ImageExtent = extent,
            ImageUsage = ImageUsageFlags.ColorAttachmentBit,
            ImageArrayLayers = 1,
            
            // TODO: Should check support for these
            ImageColorSpace = ColorSpaceKHR.SpaceSrgbNonlinearKhr,
            ImageFormat = Format.B8G8R8A8Unorm,
            PresentMode = PresentModeKHR.FifoKhr,
            
            CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
            PreTransform = SurfaceTransformFlagsKHR.IdentityBitKhr,
            Clipped = false
        };

        if (_queues.GraphicsIndex == _queues.PresentIndex)
            swapchainInfo.ImageSharingMode = SharingMode.Exclusive;
        else
            throw new NotImplementedException("Cannot support different graphics and present queues at this time.");
        
        Logger.Trace("Creating swapchain");
        _swapchainExt.CreateSwapchain(_device, &swapchainInfo, null, out _swapchain).Check("Create swapchain");

        uint numSwapchainImages;
        _swapchainExt.GetSwapchainImages(_device, _swapchain, &numSwapchainImages, null).Check("Get swapchain images");
        Image* images = stackalloc Image[(int) numSwapchainImages];
        _swapchainExt.GetSwapchainImages(_device, _swapchain, &numSwapchainImages, images).Check("Get swapchain images");

        _swapchainTextures = new VulkanTexture[numSwapchainImages];

        for (int i = 0; i < numSwapchainImages; i++)
        {
            _swapchainTextures[i] = new VulkanTexture(_vk, _device, images[i], swapchainInfo.ImageFormat,
                new Size<uint>(extent.Width, extent.Height));
        }
    }

    private void RecreateSwapchain()
    {
        Logger.Debug("Recreating swapchain");

        // CreateSwapchain uses the old swapchain during its creation, but replaces the value of _swapchain with the new
        // swapchain. Therefore, we must destroy the old swapchain only after the new one has been successfully created.
        
        SwapchainKHR currentSwapchain = _swapchain;
        
        foreach (VulkanTexture texture in _swapchainTextures)
            texture.Dispose();
        
        CreateSwapchain();
        
        _swapchainExt.DestroySwapchain(_device, currentSwapchain, null);
    }
    
    private static uint DebugCallback(DebugUtilsMessageSeverityFlagsEXT messageSeverity,
        DebugUtilsMessageTypeFlagsEXT messageTypes, DebugUtilsMessengerCallbackDataEXT* pCallbackData, void* pUserData)
    {
        string message = new string((sbyte*) pCallbackData->PMessage);

        if (messageSeverity == DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt)
            throw new Exception(message);

        string type = messageTypes.ToString().Replace("BitExt", "");

        Logger.Severity severity = messageSeverity switch
        {
            DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt => Logger.Severity.Trace,
            DebugUtilsMessageSeverityFlagsEXT.InfoBitExt => Logger.Severity.Trace,
            DebugUtilsMessageSeverityFlagsEXT.WarningBitExt => Logger.Severity.Warning,
            _ => throw new ArgumentOutOfRangeException(nameof(messageSeverity), messageSeverity, null)
        };
        
        Logger.Log(severity, $"{type}: {message}");
        
        return Vk.True;
    }
}