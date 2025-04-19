using Crimson.Graphics;
using Crimson.Graphics.Materials;

namespace Crimson.Engine.Entities.Components;

public class MeshRenderer : Component
{
    private readonly Renderable _renderable;

    public ref Material Material => ref _renderable.Material;
    
    public MeshRenderer(Mesh mesh)
    {
        _renderable = new Renderable(App.Renderer, mesh);
    }

    public override void Draw()
    {
        App.Renderer.DrawRenderable(_renderable, Transform.WorldMatrix);
    }

    public override void Dispose()
    {
        _renderable.Dispose();
    }
}