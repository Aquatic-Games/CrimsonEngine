using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace Crimson.Graphics.RHI.Vulkan.Vma
{
    // Literally copied from GRABS to save me time lol
    // https://github.com/Aquatic-Games/grabs/blob/new/src/grabs.VulkanMemoryAllocator/Vma.cs
    // VMA is one of the main reasons I am writing an RHI in crimson instead of using GRABS for the moment. I don't want
    // to deal with handling VMA correctly again. I'll deal with it later.
    public static unsafe partial class Vma
    {
        [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaCreateAllocator", ExactSpelling = true)]
        [return: NativeTypeName("VkResult")]
        public static extern Result CreateAllocator([NativeTypeName("const VmaAllocatorCreateInfo * _Nonnull")] VmaAllocatorCreateInfo* pCreateInfo, [NativeTypeName("VmaAllocator  _Nullable * _Nonnull")] VmaAllocator_T** pAllocator);
        
        [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaDestroyAllocator", ExactSpelling = true)]
        public static extern void DestroyAllocator([NativeTypeName("VmaAllocator _Nullable")] VmaAllocator_T* allocator);
        
        [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaCreateBuffer", ExactSpelling = true)]
        [return: NativeTypeName("VkResult")]
        public static extern Result CreateBuffer([NativeTypeName("VmaAllocator _Nonnull")] VmaAllocator_T* allocator, [NativeTypeName("const VkBufferCreateInfo * _Nonnull")] BufferCreateInfo* pBufferCreateInfo, [NativeTypeName("const VmaAllocationCreateInfo * _Nonnull")] VmaAllocationCreateInfo* pAllocationCreateInfo, [NativeTypeName("VkBuffer  _Nullable * _Nonnull")] Silk.NET.Vulkan.Buffer* pBuffer, [NativeTypeName("VmaAllocation  _Nullable * _Nonnull")] VmaAllocation_T** pAllocation, [NativeTypeName("VmaAllocationInfo * _Nullable")] VmaAllocationInfo* pAllocationInfo);
        
        [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaDestroyBuffer", ExactSpelling = true)]
        public static extern void DestroyBuffer([NativeTypeName("VmaAllocator _Nonnull")] VmaAllocator_T* allocator, [NativeTypeName("VkBuffer _Nullable")] Silk.NET.Vulkan.Buffer buffer, [NativeTypeName("VmaAllocation _Nullable")] VmaAllocation_T* allocation);

        [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaCreateImage", ExactSpelling = true)]
        [return: NativeTypeName("VkResult")]
        public static extern Result CreateImage([NativeTypeName("VmaAllocator _Nonnull")] VmaAllocator_T* allocator, [NativeTypeName("const VkImageCreateInfo * _Nonnull")] ImageCreateInfo* pImageCreateInfo, [NativeTypeName("const VmaAllocationCreateInfo * _Nonnull")] VmaAllocationCreateInfo* pAllocationCreateInfo, [NativeTypeName("VkImage  _Nullable * _Nonnull")] Image* pImage, [NativeTypeName("VmaAllocation  _Nullable * _Nonnull")] VmaAllocation_T** pAllocation, [NativeTypeName("VmaAllocationInfo * _Nullable")] VmaAllocationInfo* pAllocationInfo);
        
        [DllImport("VulkanMemoryAllocator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "vmaDestroyImage", ExactSpelling = true)]
        public static extern void DestroyImage([NativeTypeName("VmaAllocator _Nonnull")] VmaAllocator_T* allocator, [NativeTypeName("VkImage _Nullable")] Image image, [NativeTypeName("VmaAllocation _Nullable")] VmaAllocation_T* allocation);
    }
}
