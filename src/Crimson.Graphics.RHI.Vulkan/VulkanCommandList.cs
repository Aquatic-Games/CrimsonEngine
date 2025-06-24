using Crimson.Core;
using Silk.NET.Vulkan;

namespace Crimson.Graphics.RHI.Vulkan;

internal sealed unsafe class VulkanCommandList : CommandList
{
    private readonly Vk _vk;
    private readonly VkDevice _device;
    private readonly CommandPool _pool;

    public readonly CommandBuffer Buffer;
    
    public VulkanCommandList(Vk vk, VkDevice device, CommandPool pool)
    {
        _vk = vk;
        _device = device;
        _pool = pool;

        CommandBufferAllocateInfo allocateInfo = new()
        {
            SType = StructureType.CommandBufferAllocateInfo,
            CommandPool = _pool,
            CommandBufferCount = 1,
            Level = CommandBufferLevel.Primary
        };
        
        Logger.Trace("Allocating command buffer.");
        _vk.AllocateCommandBuffers(_device, &allocateInfo, out Buffer).Check("Allocate command buffer");
    }

    public override void Dispose()
    {
        _vk.FreeCommandBuffers(_device, _pool, 1, in Buffer);
    }
}