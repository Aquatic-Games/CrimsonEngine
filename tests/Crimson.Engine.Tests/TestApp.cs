using System.Numerics;
using Crimson.Engine;
using Crimson.Math;
using Crimson.Render;

namespace Crimson.Engine.Tests;

public class TestApp : GlobalApp
{
    private Renderable _renderable;
    
    public override void Initialize()
    {
        base.Initialize();

        Graphics graphics = App.Graphics;

        Vertex[] vertices =
        [
            new Vertex(new Vector3(-0.5f, -0.5f, 0.0f), new Vector2(0, 0), new Color(1.0f, 1.0f, 1.0f), Vector3.Zero),
            new Vertex(new Vector3(-0.5f, +0.5f, 0.0f), new Vector2(0, 1), new Color(1.0f, 1.0f, 1.0f), Vector3.Zero),
            new Vertex(new Vector3(+0.5f, +0.5f, 0.0f), new Vector2(1, 1), new Color(1.0f, 1.0f, 1.0f), Vector3.Zero),
            new Vertex(new Vector3(+0.5f, -0.5f, 0.0f), new Vector2(1, 0), new Color(1.0f, 1.0f, 1.0f), Vector3.Zero),
        ];

        uint[] indices =
        [
            0, 1, 3,
            1, 2, 3
        ];

        Mesh mesh = new Mesh(vertices, indices, new Material(graphics.CreateTexture("/home/aqua/Pictures/BAGELMIP.png")));
        _renderable = graphics.CreateRenderable(mesh);
    }

    public override void Update(float dt)
    {
        base.Update(dt);
    }

    public override void Draw()
    {
        base.Draw();
        
        App.Graphics.DrawRenderable(_renderable, Matrix4x4.Identity);
    }

    public override void Dispose()
    {
        base.Dispose();
        
        _renderable.Dispose();
    }
}