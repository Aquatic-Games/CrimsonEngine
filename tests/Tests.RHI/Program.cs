using Crimson.Core;
using Crimson.Graphics.RHI;
using Crimson.Graphics.RHI.Vulkan;
using Crimson.Math;
using Crimson.Platform;

Logger.EnableConsole();

WindowOptions options = new()
{
    Title = "Tests.RHI (Vulkan)",
    Size = new Size<int>(1280, 720),
    Resizable = true
};

bool alive = true;

Events.Create();
Events.WindowClose += () => alive = false;

Surface.Create(in options);

Device device = new VulkanDevice("Tests.RHI", Surface.Info.Handle, true);
CommandList cl = device.CreateCommandList();

while (alive)
{
    Events.ProcessEvents();

    Texture texture = device.GetNextSwapchainTexture();
    
    cl.Begin();
    
    cl.BeginRenderPass([new ColorAttachmentInfo(texture, Color.CornflowerBlue)]);
    cl.EndRenderPass();
    
    cl.End();
    
    device.ExecuteCommandList(cl);
    device.Present();
}

cl.Dispose();
device.Dispose();
Surface.Destroy();
Events.Destroy();