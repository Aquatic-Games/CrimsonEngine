using System.Numerics;
using Crimson.Engine;
using Crimson.Graphics;
using Crimson.Graphics.Materials;
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

        Renderer renderer = App.Renderer;
        renderer.Camera.ViewMatrix = Matrix4x4.CreateLookAt(new Vector3(-1, 1, 3), Vector3.Zero, Vector3.UnitY);

        Material material = new Material(renderer,
            new MaterialDefinition(new Texture(renderer, "/home/aqua/Pictures/BAGELMIP.png")));

        Model model = Model.FromGltf(renderer, "/home/aqua/Documents/test.glb");
        
        //Mesh mesh = Mesh.FromPrimitive(new Cube(), material);
        Mesh mesh = model.Meshes[0];
        _renderable = new Renderable(renderer, mesh);
    }

    public override void PreUpdate(float dt)
    {
        base.PreUpdate(dt);

        _rotation += dt;
    }

    public override void PreDraw()
    {
        base.PreDraw();
        
        App.Renderer.DrawRenderable(_renderable, Matrix4x4.CreateRotationY(_rotation));
    }

    public override void Dispose()
    {
        base.Dispose();
        
        _renderable.Dispose();
    }
}