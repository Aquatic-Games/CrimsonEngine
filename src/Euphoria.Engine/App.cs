using System.Numerics;
using Euphoria.Graphics;
using Euphoria.Math;

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
            
            Renderer.TextureBatcher.Draw(new Vector2(0, 0), new Vector2(100, 0), new Vector2(0, 100), new Vector2(100, 100), new Color(1.0f, 1.0f, 1.0f));
            
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