using System.Numerics;
using Crimson.Engine.Entities;
using Crimson.Engine.Entities.Components;
using Crimson.Graphics;
//using Crimson.Graphics.Materials;
using Crimson.Graphics.Primitives;
using Crimson.Input;
using Crimson.Math;
using Crimson.Platform;
using Hexa.NET.ImGui;
using JoltPhysicsSharp;
using Plane = Crimson.Graphics.Primitives.Plane;

namespace Crimson.Engine.Tests;

public class TestScene : Scene
{
    //private Material _material;
    //private Mesh _mesh;
    
    public override void Initialize()
    {
        App.FpsLimit = 30;
        App.Renderer.VSync = true;
        //App.Surface.CursorVisible = false;
        
        /*MaterialDefinition def = new(new Texture(App.Renderer, "DEBUG.png"))
        {
            RenderFace = RenderFace.Front
        };
        
        _material = new Material(App.Renderer, in def);
        _mesh = Mesh.FromPrimitive(new Cube(), _material);

        Entity staticCube = new Entity("StaticCube", new Transform(new Vector3(0, -5, 0)));
        staticCube.AddComponent(new Rigidbody(new BoxShape(new Vector3(0.5f)), 0));
        staticCube.AddComponent(new MeshRenderer(_mesh));
        AddEntity(staticCube);

        Entity dynamicCube = new Entity("DynamicCube");
        dynamicCube.AddComponent(new Rigidbody(new BoxShape(new Vector3(0.5f)), 1));
        dynamicCube.AddComponent(new MeshRenderer(_mesh));
        AddEntity(dynamicCube);*/
        
        Camera.Transform.Position = new Vector3(0, 0, 3);
        Camera.AddComponent(new CameraMove());
        
        /*Camera.Skybox = new Skybox(App.Renderer,
            new Bitmap("/home/aqua/Pictures/skybox/spacebox/nizzine/right.png"),
            new Bitmap("/home/aqua/Pictures/skybox/spacebox/nizzine/left.png"),
            new Bitmap("/home/aqua/Pictures/skybox/spacebox/nizzine/top.png"),
            new Bitmap("/home/aqua/Pictures/skybox/spacebox/nizzine/bottom.png"),
            new Bitmap("/home/aqua/Pictures/skybox/spacebox/nizzine/front.png"),
            new Bitmap("/home/aqua/Pictures/skybox/spacebox/nizzine/back.png"));*/
        
        base.Initialize();
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        InputManager input = App.Input;
        
        if (input.IsKeyPressed(Key.Escape))
            App.Close();

        if (input.IsMouseButtonPressed(MouseButton.Left) || input.IsMouseButtonDown(MouseButton.Right))
        {
            Entity entity = new Entity(Random.Shared.NextInt64().ToString(),
                new Transform(Camera.Transform.Position + Camera.Transform.Forward * 6));
            
            //entity.AddComponent(new MeshRenderer(_mesh));
            
            AddEntity(entity);
        }
        
        //ImGui.ShowDemoWindow();
    }
}