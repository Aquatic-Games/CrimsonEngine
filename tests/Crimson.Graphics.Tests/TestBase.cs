using System.Numerics;
using Crimson.Math;
using Crimson.Platform;
using Hexa.NET.ImGui;

namespace Crimson.Graphics.Tests;

public abstract class TestBase(string title) : IDisposable
{
    private bool _alive;
    
    protected virtual void Initialize() { }

    protected virtual void Update(float dt) { }

    protected virtual void Draw() { }

    public void Run()
    {
        Events.Create();
        Events.WindowClose += Close;
        Events.MouseMove += OnMouseMove;
        Events.MouseButtonDown += OnMouseButtonDown;
        Events.MouseButtonUp += OnMouseButtonUp;
        
        Surface.Create(new WindowOptions(title, new Size<int>(1280, 720), false, false));
        Renderer.Create(title, new RendererOptions(RendererType.CreateBoth, true, true), Surface.Info);

        Surface.Details = $" - {Renderer.Backend}";
        
        Initialize();

        _alive = true;
        while (_alive)
        {
            Events.ProcessEvents();
            Renderer.NewFrame();
            
            Update(1.0f / 60.0f);
            Draw();
            
            Renderer.Render();
        }
    }

    public void Close()
    {
        _alive = false;
    }

    public virtual void Dispose()
    {
        Renderer.Destroy();
        Surface.Destroy();
        Events.Destroy();
    }
    
    private void OnMouseMove(Vector2 position, Vector2 delta)
    {
        ImGuiIOPtr io = ImGui.GetIO();
        io.AddMousePosEvent(position.X, position.Y);
    }
    
    private void OnMouseButtonDown(MouseButton button)
    {
        ImGuiIOPtr io = ImGui.GetIO();
        io.AddMouseButtonEvent((int) ToImGuiMouseButton(button), true);
    }
    
    private void OnMouseButtonUp(MouseButton button)
    {
        ImGuiIOPtr io = ImGui.GetIO();
        io.AddMouseButtonEvent((int) ToImGuiMouseButton(button), false);
    }

    private static ImGuiMouseButton ToImGuiMouseButton(MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => ImGuiMouseButton.Left,
            MouseButton.Middle => ImGuiMouseButton.Middle,
            MouseButton.Right => ImGuiMouseButton.Right,
            _ => ImGuiMouseButton.Middle
        };
    }
}