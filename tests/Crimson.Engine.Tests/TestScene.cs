using System.Numerics;
using Crimson.Engine.Entities;
using Crimson.Engine.Entities.Components;
using Crimson.Graphics;
using Crimson.Graphics.Materials;
using Crimson.Graphics.Primitives;
using Crimson.Input;
using Crimson.Platform;
using JoltPhysicsSharp;
using Plane = Crimson.Graphics.Primitives.Plane;

namespace Crimson.Engine.Tests;

public class TestScene : Scene
{
    public override void Initialize()
    {
        App.FpsLimit = 30;
        App.Renderer.VSync = true;
        
        MaterialDefinition def = new(new Texture(App.Renderer, "DEBUG.png"))
        {
            RenderFace = RenderFace.Both
        };
        
        Material material = new Material(App.Renderer, in def);

        Entity staticCube = new Entity("StaticCube", new Transform(new Vector3(0, -5, 0)));
        staticCube.AddComponent(new Rigidbody(new BoxShape(new Vector3(0.5f)), 0));
        staticCube.AddComponent(new MeshRenderer(Mesh.FromPrimitive(new Cube(), material)));
        AddEntity(staticCube);

        Entity dynamicCube = new Entity("DynamicCube");
        dynamicCube.AddComponent(new Rigidbody(new BoxShape(new Vector3(0.5f)), 1));
        dynamicCube.AddComponent(new MeshRenderer(Mesh.FromPrimitive(new Cube(), material)));
        AddEntity(dynamicCube);
        
        Camera.Transform.Position = new Vector3(0, 0, 3);
        Camera.AddComponent(new CameraMove());

        Bitmap bitmap = new Bitmap("DEBUG.png");
        Camera.Skybox = new Skybox(App.Renderer, [bitmap, bitmap, bitmap, bitmap, bitmap, bitmap]);
        
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