using Silk.NET.Vulkan;

namespace Euphoria.Graphics.Vulkan.RenderAbstraction;

internal static class VkUtils
{
    public static void Check(this Result result, string operation)
    {
        if (result != Result.Success)
            throw new VulkanOperationException(operation, result);
    }
}