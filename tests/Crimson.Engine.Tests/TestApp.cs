using System.Numerics;
using Crimson.Engine;
using Crimson.Math;
using Crimson.Render;

namespace Crimson.Engine.Tests;

public class TestApp : GlobalApp
{
    private Renderable _renderable;

    private float _rotation;
    
    public override void Initialize()
    {
        base.Initialize();

        Graphics graphics = App.Graphics;
        graphics.Camera.ViewMatrix = Matrix4x4.CreateLookAt(new Vector3(-1, 1, 3), Vector3.Zero, Vector3.UnitY);

        Vertex[] vertices =
        [
            new Vertex(new Vector3(-0.5f, -0.5f, 0.0f), new Vector2(0, 1), new Color(1.0f, 1.0f, 1.0f), Vector3.Zero),
            new Vertex(new Vector3(-0.5f, +0.5f, 0.0f), new Vector2(0, 0), new Color(1.0f, 1.0f, 1.0f), Vector3.Zero),
            new Vertex(new Vector3(+0.5f, +0.5f, 0.0f), new Vector2(1, 0), new Color(1.0f, 1.0f, 1.0f), Vector3.Zero),
            new Vertex(new Vector3(+0.5f, -0.5f, 0.0f), new Vector2(1, 1), new Color(1.0f, 1.0f, 1.0f), Vector3.Zero),
        ];

        uint[] indices =
        [
            0, 1, 3,
            1, 2, 3
        ];

        Material material =
            graphics.CreateMaterial(
                new MaterialDefinition(graphics.CreateTexture("C:/Users/aqua/Pictures/BAGELMIP.png")));

        Mesh mesh = new Mesh(vertices, indices, material);
        _renderable = graphics.CreateRenderable(mesh);
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        _rotation += dt;
    }

    public override void Draw()
    {
        base.Draw();
        
        App.Graphics.DrawRenderable(_renderable, Matrix4x4.CreateRotationY(_rotation));
    }

    public override void Dispose()
    {
        base.Dispose();
        
        _renderable.Dispose();
    }
}