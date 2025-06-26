namespace Crimson.Graphics.RHI.Vulkan.Vma
{
    public unsafe partial struct VmaAllocationCreateInfo
    {
        [NativeTypeName("VmaAllocationCreateFlags")]
        public uint flags;

        public VmaMemoryUsage usage;

        [NativeTypeName("VkMemoryPropertyFlags")]
        public uint requiredFlags;

        [NativeTypeName("VkMemoryPropertyFlags")]
        public uint preferredFlags;

        [NativeTypeName("uint32_t")]
        public uint memoryTypeBits;

        [NativeTypeName("VmaPool _Nullable")]
        public void* pool;

        [NativeTypeName("void * _Nullable")]
        public void* pUserData;

        public float priority;
    }
}
