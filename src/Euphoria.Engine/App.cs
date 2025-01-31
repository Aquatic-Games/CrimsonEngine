using Euphoria.Engine.Launch;

namespace Euphoria.Engine;

public class App : IDisposable
{
    public readonly Window Window;

    public bool IsAlive;
    
    public App(in LaunchInfo info)
    {
        Window = new Window(info.Window);
        Window.Close += Close;
    }

    public void Run()
    {
        IsAlive = true;

        while (IsAlive)
        {
            Window.ProcessEvents();
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