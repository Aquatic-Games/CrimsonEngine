global using VkFormat = Silk.NET.Vulkan.Format;
using Silk.NET.Vulkan;

namespace Crimson.Graphics.RHI.Vulkan;

internal static class VulkanUtils
{
    public static VkFormat ToVk(this Format format)
    {
        return format switch
        {
            Format.R8B8B8A8_UNorm => VkFormat.R8G8B8A8Unorm,
            Format.B8G8R8A8_UNorm => VkFormat.B8G8R8A8Unorm,
            Format.R32_Float => VkFormat.R32Sfloat,
            Format.R32G32_Float => VkFormat.R32G32Sfloat,
            Format.R32G32B32_Float => VkFormat.R32G32B32Sfloat,
            Format.R32G32B32A32_Float => VkFormat.R32G32B32A32Sfloat,
            Format.R16_UInt => VkFormat.R16Uint,
            Format.R32_UInt => VkFormat.R32Uint,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }
    
    public static AttachmentLoadOp ToVk(this LoadOp op)
    {
        return op switch
        {
            LoadOp.Clear => AttachmentLoadOp.Clear,
            LoadOp.Load => AttachmentLoadOp.Load,
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
        };
    }

    public static AttachmentStoreOp ToVk(this StoreOp op)
    {
        return op switch
        {
            StoreOp.Store => AttachmentStoreOp.Store,
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
        };
    }
}