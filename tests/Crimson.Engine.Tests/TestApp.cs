using System.Numerics;
using Crimson.Engine;
using Crimson.Graphics;
using Crimson.Graphics.Materials;
//using Crimson.Graphics.Materials;
using Crimson.Math;
using Plane = Crimson.Graphics.Primitives.Plane;

namespace Crimson.Engine.Tests;

public class TestApp : GlobalApp
{
    private Renderable _renderable;

    private float _rotation;
    
    public override void Initialize()
    {
        base.Initialize();
        
        Renderer.Camera.ViewMatrix = Matrix4x4.CreateLookAt(new Vector3(-1, 1, 3), Vector3.Zero, Vector3.UnitY);

        Material material = new StandardLit(new MaterialDefinition(new Texture("/home/aqua/Pictures/BAGELMIP.png")));
        
        
        //Mesh mesh = Mesh.FromPrimitive(new Cube(), material);
        //_renderable = new Renderable(renderer, mesh);
    }

    public override void PreUpdate(float dt)
    {
        base.PreUpdate(dt);

        _rotation += dt;
    }

    public override void PreDraw()
    {
        base.PreDraw();
        
        //App.Renderer.DrawRenderable(_renderable, Matrix4x4.CreateRotationY(_rotation));
    }

    public override void Dispose()
    {
        base.Dispose();
        
        //_renderable.Dispose();
    }
}