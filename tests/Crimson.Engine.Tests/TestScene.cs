using System.Numerics;
using Crimson.Engine.Entities;
using Crimson.Engine.Entities.Components;
using Crimson.Input;
using Crimson.Platform;
using Crimson.Render;
using Crimson.Render.Primitives;

namespace Crimson.Engine.Tests;

public class TestScene : Scene
{
    public override void Initialize()
    {
        Material material = new Material(App.Graphics,
            new MaterialDefinition(new Texture(App.Graphics, "DEBUG.png")));
        
        Entity test = new Entity("test", new Transform() { Scale = new Vector3(2, 1, 1), Origin = new Vector3(1, 0, 0) });
        test.AddComponent(new MeshRenderer(Mesh.FromPrimitive(new Cube(), material)));
        test.AddComponent(new TestComponent());
        AddEntity(test);

        Entity test2 = new Entity("test2", new Transform() { Position = new Vector3(-2, 0, 0) });
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