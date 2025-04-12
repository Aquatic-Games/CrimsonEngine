using Crimson.Render;
using Crimson.Render.Materials;

namespace Crimson.Engine.Entities.Components;

public class MeshRenderer : Component
{
    private readonly Renderable _renderable;

    public ref Material Material => ref _renderable.Material;
    
    public MeshRenderer(Mesh mesh)
    {
        _renderable = new Renderable(App.Graphics, mesh);
    }

    public override void Draw()
    {
        App.Graphics.DrawRenderable(_renderable, Transform.WorldMatrix);
    }

    public override void Dispose()
    {
        _renderable.Dispose();
    }
}