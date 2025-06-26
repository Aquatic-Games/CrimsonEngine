using Silk.NET.Vulkan;

namespace Crimson.Graphics.RHI.Vulkan.Vma
{
    public unsafe partial struct VmaAllocationInfo
    {
        [NativeTypeName("uint32_t")]
        public uint memoryType;

        [NativeTypeName("VkDeviceMemory _Nullable")]
        public DeviceMemory deviceMemory;

        [NativeTypeName("VkDeviceSize")]
        public nuint offset;

        [NativeTypeName("VkDeviceSize")]
        public nuint size;

        [NativeTypeName("void * _Nullable")]
        public void* pMappedData;

        [NativeTypeName("void * _Nullable")]
        public void* pUserData;

        [NativeTypeName("const char * _Nullable")]
        public sbyte* pName;
    }
}
