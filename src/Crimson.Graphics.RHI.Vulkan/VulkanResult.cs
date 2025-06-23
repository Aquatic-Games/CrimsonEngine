using Silk.NET.Vulkan;

namespace Crimson.Graphics.RHI.Vulkan;

internal static class VulkanResult
{
    /// <summary>
    /// Checks a Vulkan <see cref="Result"/>, throwing an exception if not <see cref="Result.Success"/>.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="operation">The operation name, given in the exception if thrown.</param>
    public static void Check(this Result result, string operation)
    {
        // TODO: VulkanOperationException
        if (result != Result.Success)
            throw new Exception($"Vulkan operation '{operation}' failed with result: {result}");
    }
}