using Silk.NET.Vulkan;

namespace Crimson.Graphics.RHI.Vulkan;

internal struct Queues
{
    public uint GraphicsIndex;
    public Queue Graphics;

    public uint PresentIndex;
    public Queue Present;
}