global using VkShaderModule = Silk.NET.Vulkan.ShaderModule;
using Crimson.Core;
using Silk.NET.Vulkan;

namespace Crimson.Graphics.RHI.Vulkan;

internal sealed unsafe class VulkanShaderModule : ShaderModule
{
    private readonly Vk _vk;
    private readonly VkDevice _device;
    
    public readonly VkShaderModule Module;

    public readonly string EntryPoint;

    public VulkanShaderModule(Vk vk, VkDevice device, ShaderStage stage, in ReadOnlySpan<byte> spirv, string entryPoint)
    {
        _vk = vk;
        _device = device;
        EntryPoint = entryPoint;

        fixed (byte* pSpirv = spirv)
        {
            ShaderModuleCreateInfo moduleInfo = new()
            {
                SType = StructureType.ShaderModuleCreateInfo,
                CodeSize = (uint) spirv.Length,
                PCode = (uint*) pSpirv
            };
            
            Logger.Trace("Creating shader module");
            _vk.CreateShaderModule(_device, &moduleInfo, null, out Module).Check("Create shader module");
        }
    }
    
    public override void Dispose()
    {
        _vk.DestroyShaderModule(_device, Module, null);
    }
}