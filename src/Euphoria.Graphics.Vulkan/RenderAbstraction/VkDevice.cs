using Euphoria.Core;
using SDL;
using Silk.NET.Vulkan;

namespace Euphoria.Graphics.Vulkan.RenderAbstraction;

internal unsafe class VkDevice : IDisposable
{
    public readonly Vk Vk;

    public readonly Instance Instance;
    public readonly Device Device;

    public VkDevice()
    {
        Vk = Vk.GetApi();

        uint numExtensions;
        byte** extensions = SDL3.SDL_Vulkan_GetInstanceExtensions(&numExtensions);

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