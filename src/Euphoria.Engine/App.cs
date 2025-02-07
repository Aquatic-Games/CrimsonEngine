using Euphoria.Graphics;

namespace Euphoria.Engine;

public class App : IDisposable
{
    public readonly Window Window;

    public readonly Renderer Renderer;

    public bool IsAlive;
    
    public unsafe App(in LaunchInfo info)
    {
        Window = new Window(info.Window);
        Window.Close += Close;

        Renderer = new Renderer(Window.Handle, info.Renderer);
    }

    public void Run()
    {
        IsAlive = true;

        while (IsAlive)
        {
            Window.ProcessEvents();
            
            Renderer.Render();
        }
    }

    public void Close()
    {
        IsAlive = false;
    }
    
    public void Dispose()
    {
        Renderer.Dispose();
        Window.Dispose();
    }
}