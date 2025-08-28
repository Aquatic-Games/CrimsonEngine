using Crimson.Math;
using Crimson.Platform;

namespace Crimson.Graphics.Tests;

public abstract class TestBase : IDisposable
{
    public readonly string Name;
    
    protected TestBase(string name)
    {
        Name = name;
    }

    protected virtual void Initialize() { }

    protected virtual void Update(float dt) { }

    protected virtual void Draw() { }

    public void Run()
    {
        bool alive = true;
        
        Events.Create();
        Events.WindowClose += () => alive = false;
        
        Surface.Create(new WindowOptions(Name, new Size<int>(1280, 720), false, false));
        Renderer.Create(Name, new RendererOptions(RendererType.CreateBoth, true, true), Surface.Info, Surface.Size);

        Initialize();
        
        while (alive)
        {
            Events.ProcessEvents();
            
            Update(1 / 60.0f);
            Draw();
            
            Renderer.Render();
        }
    }

    public virtual void Dispose()
    {
        Renderer.Destroy();
        Surface.Destroy();
        Events.Destroy();
    }
}