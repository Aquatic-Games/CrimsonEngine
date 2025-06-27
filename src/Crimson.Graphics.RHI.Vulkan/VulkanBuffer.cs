global using VkBuffer = Silk.NET.Vulkan.Buffer;
using Crimson.Core;
using Crimson.Graphics.RHI.Vulkan.Vma;
using Silk.NET.Vulkan;
using static Crimson.Graphics.RHI.Vulkan.Vma.VmaAllocationCreateFlagBits;
using static Crimson.Graphics.RHI.Vulkan.Vma.VmaMemoryUsage;

namespace Crimson.Graphics.RHI.Vulkan;

internal unsafe class VulkanBuffer : Buffer
{
    private readonly Vk _vk;
    private readonly VkDevice _device;
    private readonly VmaAllocator_T* _allocator;
    
    public readonly VmaAllocation_T* Allocation;
    public readonly VkBuffer Buffer;

    public VulkanBuffer(Vk vk, VkDevice device, VmaAllocator_T* allocator, BufferUsage usage, uint sizeInBytes)
    {
        _vk = vk;
        _device = device;
        _allocator = allocator;

        VmaAllocationCreateInfo allocInfo = new()
        {
            usage = VMA_MEMORY_USAGE_AUTO
        };
        
        BufferUsageFlags flags = 0;

        if ((usage & BufferUsage.VertexBuffer) != 0)
            flags |= BufferUsageFlags.VertexBufferBit;
        if ((usage & BufferUsage.IndexBuffer) != 0)
            flags |= BufferUsageFlags.IndexBufferBit;
        if ((usage & BufferUsage.ConstantBuffer) != 0)
            flags |= BufferUsageFlags.UniformBufferBit;

        if ((usage & BufferUsage.TransferDst) != 0)
            flags |= BufferUsageFlags.TransferDstBit;
        if ((usage & BufferUsage.TransferSrc) != 0)
        {
            flags |= BufferUsageFlags.TransferSrcBit;
            allocInfo.flags |= (uint) VMA_ALLOCATION_CREATE_HOST_ACCESS_SEQUENTIAL_WRITE_BIT;
        }

        BufferCreateInfo bufferInfo = new()
        {
            SType = StructureType.BufferCreateInfo,
            Usage = flags,
            Size = sizeInBytes
        };

        Logger.Trace("Creating buffer.");
        Vma.Vma.CreateBuffer(_allocator, &bufferInfo, &allocInfo, out Buffer, out Allocation, out _);
    }
    
    public override void Dispose()
    {
        Vma.Vma.DestroyBuffer(_allocator, Buffer, Allocation);
    }
}