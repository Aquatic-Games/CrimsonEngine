using Crimson.Core;
using Crimson.Graphics.RHI;
using Crimson.Graphics.RHI.Vulkan;
using Crimson.Math;
using Crimson.Platform;

Logger.EnableConsole();

WindowOptions options = new()
{
    Title = "Tests.RHI (Vulkan)",
    Size = new Size<int>(1280, 720)
};

Surface.Create(in options);
Device device = new VulkanDevice("Tests.RHI", Surface.Info.Handle);

device.Dispose();
Surface.Destroy();