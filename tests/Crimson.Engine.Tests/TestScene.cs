using System.Numerics;
using Crimson.Engine.Entities;
using Crimson.Engine.Entities.Components;
using Crimson.Input;
using Crimson.Platform;
using Crimson.Render;
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
        
        /*MaterialDefinition def = new(new Texture(App.Graphics, "DEBUG.png"))
        {
            RenderFace = RenderFace.Both
        };
        
        Material material = new Material(App.Graphics, in def);

        Entity staticCube = new Entity("StaticCube", new Transform(new Vector3(0, -5, 0)));
        staticCube.AddComponent(new Rigidbody(new BoxShape(new Vector3(0.5f)), 0));
        staticCube.AddComponent(new MeshRenderer(Mesh.FromPrimitive(new Cube(), material)));
        AddEntity(staticCube);

        Entity dynamicCube = new Entity("DynamicCube");
        dynamicCube.AddComponent(new Rigidbody(new BoxShape(new Vector3(0.5f)), 1));
        dynamicCube.AddComponent(new MeshRenderer(Mesh.FromPrimitive(new Cube(), material)));
        AddEntity(dynamicCube);
        
        Camera.Transform.Position = new Vector3(0, 0, 3);
        Camera.AddComponent(new CameraMove());*/
        
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