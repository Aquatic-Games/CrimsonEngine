using Euphoria.Engine.Launch;
using Euphoria.Graphics;
using Euphoria.Graphics.Vulkan;

namespace Euphoria.Engine;

public class App : IDisposable
{
    public readonly Window Window;

    public readonly Renderer Renderer;

    public bool IsAlive;
    
    public App(in LaunchInfo info)
    {
        Window = new Window(info.Window);
        Window.Close += Close;

        Renderer = new VkRenderer();
    }

    public void Run()
    {
        IsAlive = true;

        while (IsAlive)
        {
            Window.ProcessEvents();
            
            Renderer.Present();
        }
    }

    public void Close()
    {
        IsAlive = false;
    }
    
    public void Dispose()
    {
        Window.Dispose();
    }
}