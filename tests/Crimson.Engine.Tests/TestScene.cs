using System.Numerics;
using Crimson.Engine.Entities;
using Crimson.Engine.Entities.Components;
using Crimson.Graphics;
using Crimson.Graphics.Materials;
//using Crimson.Graphics.Materials;
using Crimson.Graphics.Primitives;
using Crimson.Input;
using Crimson.Math;
using Crimson.Platform;
using Hexa.NET.ImGui;
using JoltPhysicsSharp;
using SharpGLTF.Schema2;
using Material = Crimson.Graphics.Materials.Material;
using Mesh = Crimson.Graphics.Mesh;
using Plane = Crimson.Graphics.Primitives.Plane;
using Scene = Crimson.Engine.Entities.Scene;
using Texture = Crimson.Graphics.Texture;

namespace Crimson.Engine.Tests;

public class TestScene : Scene
{
    private Texture _texture;
    private Texture _texture2;
    private Material _material;
    private Mesh _mesh;
    
    public override void Initialize()
    {
        //App.FpsLimit = 240;
        //App.Renderer.VSync = false;
        //App.Surface.CursorVisible = false;

        _texture = new Texture(App.Renderer, "DEBUG.png");
        //_texture2 = new Texture(App.Renderer, "/home/aqua/Pictures/awesomeface.png");
        
        MaterialDefinition def = new(_texture)
        {
            RenderFace = RenderFace.Front
        };
        
        _material = new Material(App.Renderer, in def);
        _mesh = Mesh.FromPrimitive(new Cube(), _material);
        
        Model model = Model.FromGltf(App.Renderer, "/home/aqua/Downloads/Fox.glb");

        Entity mainCube = new Entity("MainCube", new Transform() { Scale = new Vector3(0.1f) });
        model.AddToEntity(mainCube);

        Entity secondCube = new Entity("Cube1", new Transform(new Vector3(1, 0, 0)));
        secondCube.AddComponent(new MeshRenderer(_mesh));
        mainCube.AddChild(secondCube);
        
        AddEntity(mainCube);
        
        Camera.Transform.Position = new Vector3(0, 0, 3);
        Camera.AddComponent(new CameraMove());
        
        Camera.Skybox = new Skybox(App.Renderer,
            new Bitmap("/home/aqua/Pictures/skybox/spacebox/nizzine/right.png"),
            new Bitmap("/home/aqua/Pictures/skybox/spacebox/nizzine/left.png"),
            new Bitmap("/home/aqua/Pictures/skybox/spacebox/nizzine/top.png"),
            new Bitmap("/home/aqua/Pictures/skybox/spacebox/nizzine/bottom.png"),
            new Bitmap("/home/aqua/Pictures/skybox/spacebox/nizzine/front.png"),
            new Bitmap("/home/aqua/Pictures/skybox/spacebox/nizzine/back.png"));
        
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
            
            entity.AddComponent(new MeshRenderer(_mesh));
            
            AddEntity(entity);
        }

        GetEntity("MainCube").Transform.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, dt);
        GetEntity("MainCube/Cube1").Transform.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitX, dt);
    }

    public override void Draw()
    {
        base.Draw();
        
        //App.Renderer.DrawImage(_texture2, Vector2.Zero);
        //App.Renderer.DrawImage(_texture, Vector2.Zero);
        //App.Renderer.DrawImage(_texture, new Vector2(100));
        
        Console.WriteLine("Egg");
        
        App.Renderer.DrawLine(new Vector2(0, 0), new Vector2(1280, 720), Color.White, 5);
    }
}