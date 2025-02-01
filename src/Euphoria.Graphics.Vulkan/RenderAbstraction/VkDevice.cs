using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Euphoria.Core;
using SDL;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;

namespace Euphoria.Graphics.Vulkan.RenderAbstraction;

internal unsafe class VkDevice : IDisposable
{
    public readonly Vk Vk;

    public readonly Instance Instance;
    public readonly Device Device;

    public VkDevice(bool debug)
    {
        Vk = Vk.GetApi();

        uint numExtensions;
        byte** extensions = SDL3.SDL_Vulkan_GetInstanceExtensions(&numExtensions);

        uint numLayers = 0;
        byte** layers = null;

        if (debug)
        {
            Logger.Warn("Graphics debugging enabled. This will impact performance.");
            
            byte** sdlExtensions = extensions;
            
            extensions = (byte**) NativeMemory.Alloc((nuint) ((numExtensions + 1) * sizeof(byte*)));
            
            Unsafe.CopyBlock(extensions, sdlExtensions, (uint) (numExtensions * sizeof(byte*)));
            
            byte[] extBytes = Encoding.UTF8.GetBytes(ExtDebugUtils.ExtensionName);

            extensions[numExtensions] = (byte*) NativeMemory.Alloc((nuint) ((extBytes.Length + 1) * sizeof(byte)));
            
            fixed (byte* pExtBytes = extBytes)
                Unsafe.CopyBlock(extensions + numExtensions, pExtBytes, (uint) extBytes.Length);
            
            extensions[numExtensions][extBytes.Length] = 0;

            numLayers = 1;
            layers = (byte**) NativeMemory.Alloc((nuint) (1 * sizeof(byte*)));

            ReadOnlySpan<byte> layer = "VK_LAYER_KHRONOS_validation"u8;

            layers[0] = (byte*) NativeMemory.Alloc((nuint) ((layer.Length + 1) * sizeof(byte)));

            fixed (byte* pLayer = layer)
                Unsafe.CopyBlock(layers, pLayer, (uint) layer.Length);

            layers[0][layer.Length] = 0;
        }

        fixed (byte* pEngineName = "Euphoria"u8)
        {
            ApplicationInfo appInfo = new ApplicationInfo()
            {
                SType = StructureType.ApplicationInfo,

                PEngineName = pEngineName,
                EngineVersion = Vk.MakeVersion(1, 0, 0),

                ApiVersion = Vk.Version13
            };

            InstanceCreateInfo instanceInfo = new InstanceCreateInfo()
            {
                SType = StructureType.InstanceCreateInfo,
                PApplicationInfo = &appInfo,

                PpEnabledExtensionNames = extensions,
                EnabledExtensionCount = numExtensions,
                
                PpEnabledLayerNames = layers,
                EnabledLayerCount = numLayers
            };

            Logger.Trace("Creating instance.");
            Vk.CreateInstance(&instanceInfo, null, out Instance).Check("Create instance");
        }
    }
    
    public void Dispose()
    {
        Vk.DestroyInstance(Instance, null);
        
        Vk.Dispose();
    }
}