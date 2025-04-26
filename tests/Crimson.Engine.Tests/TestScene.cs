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
        App.Surface.CursorVisible = false;

        _texture = new Texture(App.Renderer, "DEBUG.png");
        _texture2 = new Texture(App.Renderer, "C:/Users/aqua/Pictures/awesomeface.png");
        
        MaterialDefinition def = new(_texture)
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
        AddEntity(dynamicCube);
        
        Camera.Transform.Position = new Vector3(0, 0, 3);
        Camera.AddComponent(new CameraMove());
        
        Camera.Skybox = new Skybox(App.Renderer,
            new Bitmap("C:/Users/aqua/Pictures/skybox/spacebox/nizzine/right.png"),
            new Bitmap("C:/Users/aqua/Pictures/skybox/spacebox/nizzine/left.png"),
            new Bitmap("C:/Users/aqua/Pictures/skybox/spacebox/nizzine/top.png"),
            new Bitmap("C:/Users/aqua/Pictures/skybox/spacebox/nizzine/bottom.png"),
            new Bitmap("C:/Users/aqua/Pictures/skybox/spacebox/nizzine/front.png"),
            new Bitmap("C:/Users/aqua/Pictures/skybox/spacebox/nizzine/back.png"));
        
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
        
        //ImGui.ShowDemoWindow();
    }

    public override void Draw()
    {
        base.Draw();
        
        App.Renderer.DrawImage(_texture2, Vector2.Zero);
        App.Renderer.DrawImage(_texture, Vector2.Zero);
        App.Renderer.DrawImage(_texture, new Vector2(100));
    }
}