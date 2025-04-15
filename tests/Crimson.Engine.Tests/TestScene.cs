using System.Numerics;
using Crimson.Engine.Entities;
using Crimson.Engine.Entities.Components;
using Crimson.Input;
using Crimson.Platform;
using Crimson.Render;
using Crimson.Render.Materials;
using Crimson.Render.Primitives;
using JoltPhysicsSharp;
using Plane = Crimson.Render.Primitives.Plane;

namespace Crimson.Engine.Tests;

public class TestScene : Scene
{
    public override void Initialize()
    {
        App.FpsLimit = 30;
        App.Graphics.VSync = true;
        
        MaterialDefinition def = new(new Texture(App.Graphics, "DEBUG.png"))
        {
            RenderFace = RenderFace.Both
        };
        
        Material material = new Material(App.Graphics, in def);
        
        Entity test = new Entity("test", new Transform() { Scale = new Vector3(5, 1, 1), Origin = new Vector3(1, 0, 0) });
        test.AddComponent(new MeshRenderer(Mesh.FromPrimitive(new Plane(), material)));
        test.AddComponent(new TestComponent());
        AddEntity(test);

        Entity test2 = new Entity("test2", new Transform() { Position = new Vector3(-2, 0, 0) });
        test2.AddComponent(new Rigidbody(new BoxShape(new Vector3(0.5f))));
        test2.AddComponent(new MeshRenderer(Mesh.FromPrimitive(new Cube(), material)));
        AddEntity(test2);
        
        Entity test3 = new Entity("test3", new Transform() { Position = new Vector3(2, 0, 0) });
        test3.AddComponent(new MeshRenderer(Mesh.FromPrimitive(new Cube(), material)));
        AddEntity(test3);
        
        Camera.Transform.Position = new Vector3(0, 0, 3);
        Camera.AddComponent(new CameraMove());
        
        base.Initialize();
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        InputManager input = App.Input;
        
        if (input.IsKeyDown(Key.Space))
            Console.WriteLine("Yes");
        else
            Console.WriteLine("No");
        
        if (input.IsKeyPressed(Key.S))
            Console.WriteLine("Pressed");
        
        if (input.IsKeyPressed(Key.Escape))
            App.Close();
    }
}