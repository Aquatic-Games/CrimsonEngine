using Silk.NET.Vulkan;

namespace Crimson.Graphics.RHI.Vulkan;

internal static class VulkanUtils
{
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