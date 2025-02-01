using Silk.NET.Vulkan;

namespace Euphoria.Graphics.Vulkan.RenderAbstraction;

public class VulkanOperationException(string operation, Result result)
    : Exception($"Vulkan operation '{operation}' failed with result: {result}");