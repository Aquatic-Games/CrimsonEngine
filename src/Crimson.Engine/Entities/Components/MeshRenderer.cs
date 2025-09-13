using Crimson.Graphics;
using Crimson.Graphics.Materials;

namespace Crimson.Engine.Entities.Components;

public class MeshRenderer : Component
{
    private readonly Renderable _renderable;

    public ref Material Material => ref _renderable.Material;
    
    public MeshRenderer(Mesh mesh)
    {
        _renderable = new Renderable(mesh);
    }

    public override void Draw()
    {
        Renderer.DrawRenderable(_renderable, Entity.WorldMatrix);
    }

    public override void Dispose()
    {
        _renderable.Dispose();
    }
}