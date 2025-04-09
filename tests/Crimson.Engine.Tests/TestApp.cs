using System.Numerics;
using Crimson.Engine;
using Crimson.Math;
using Crimson.Render;
using Crimson.Render.Primitives;
using Plane = Crimson.Render.Primitives.Plane;

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

        Material material = new Material(graphics,
            new MaterialDefinition(new Texture(graphics, "C:/Users/aqua/Pictures/BAGELMIP.png")));

        Mesh mesh = Mesh.FromPrimitive(new Cube(), material);
        _renderable = new Renderable(graphics, mesh);
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