namespace Crimson.Graphics.RHI.Vulkan.Vma
{
    [NativeTypeName("unsigned int")]
    public enum VmaMemoryUsage : uint
    {
        VMA_MEMORY_USAGE_UNKNOWN = 0,
        VMA_MEMORY_USAGE_GPU_ONLY = 1,
        VMA_MEMORY_USAGE_CPU_ONLY = 2,
        VMA_MEMORY_USAGE_CPU_TO_GPU = 3,
        VMA_MEMORY_USAGE_GPU_TO_CPU = 4,
        VMA_MEMORY_USAGE_CPU_COPY = 5,
        VMA_MEMORY_USAGE_GPU_LAZILY_ALLOCATED = 6,
        VMA_MEMORY_USAGE_AUTO = 7,
        VMA_MEMORY_USAGE_AUTO_PREFER_DEVICE = 8,
        VMA_MEMORY_USAGE_AUTO_PREFER_HOST = 9,
        VMA_MEMORY_USAGE_MAX_ENUM = 0x7FFFFFFF,
    }
}
