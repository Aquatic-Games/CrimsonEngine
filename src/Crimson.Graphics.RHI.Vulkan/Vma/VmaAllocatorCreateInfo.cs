using Silk.NET.Vulkan;

namespace Crimson.Graphics.RHI.Vulkan.Vma
{
    public unsafe partial struct VmaAllocatorCreateInfo
    {
        [NativeTypeName("VmaAllocatorCreateFlags")]
        public uint flags;

        [NativeTypeName("VkPhysicalDevice _Nonnull")]
        public PhysicalDevice physicalDevice;

        [NativeTypeName("VkDevice _Nonnull")]
        public VkDevice device;

        [NativeTypeName("VkDeviceSize")]
        public nuint preferredLargeHeapBlockSize;

        [NativeTypeName("const VkAllocationCallbacks * _Nullable")]
        public AllocationCallbacks* pAllocationCallbacks;

        [NativeTypeName("const VmaDeviceMemoryCallbacks * _Nullable")]
        public void* pDeviceMemoryCallbacks;

        [NativeTypeName("const VkDeviceSize * _Nullable")]
        public nuint* pHeapSizeLimit;

        [NativeTypeName("const VmaVulkanFunctions * _Nullable")]
        public void* pVulkanFunctions;

        [NativeTypeName("VkInstance _Nonnull")]
        public Instance instance;

        [NativeTypeName("uint32_t")]
        public uint vulkanApiVersion;

        [NativeTypeName("const VkExternalMemoryHandleTypeFlagsKHR * _Nullable")]
        public uint* pTypeExternalMemoryHandleTypes;
    }
}
