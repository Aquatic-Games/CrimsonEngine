using Crimson.Math;
using Crimson.Platform;

namespace Crimson.Graphics.Tests;

public abstract class TestBase(string title) : IDisposable
{
    protected virtual void Initialize() { }

    protected virtual void Update(float dt) { }

    protected virtual void Draw() { }

    public void Run()
    {
        Events.Create();
        Surface.Create(new WindowOptions(title, new Size<int>(1280, 720), false, false));
        Renderer.Create(title, new RendererOptions(RendererType.CreateBoth, true, true), Surface.Info);

        bool alive = true;
        Events.WindowClose += () => alive = false; 
        
        Initialize();

        while (alive)
        {
            Events.ProcessEvents();
            Renderer.NewFrame();
            
            Update(1.0f / 60.0f);
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