using System.Numerics;
using Crimson.Data;
using Crimson.Engine.Entities;
using Crimson.Engine.Entities.Components;
using Crimson.Graphics;
using Crimson.Graphics.Materials;
//using Crimson.Graphics.Materials;
using Crimson.Graphics.Primitives;
using Crimson.Math;
using Crimson.Physics;
using Crimson.Physics.Shapes;
using Crimson.Physics.Shapes.Descriptions;
using Crimson.Platform;
using Hexa.NET.ImGui;
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
    private Material _material;
    private Mesh _mesh;
    
    public override void Initialize()
    {
        //App.FpsLimit = 240;
        //App.Renderer.VSync = false;
        //App.Surface.CursorVisible = false;

        _texture = Content.Content.Load<Texture>("awesomeface");
        
        MaterialDefinition def = new(_texture)
        {
            RenderFace = RenderFace.Front
        };
        
        _material = new Material(in def);
        _mesh = Mesh.FromPrimitive(new Cube(), _material);
        
        Model model = Model.FromGltf("/home/aqua/Downloads/Fox.glb");

        BoxShapeDescription desc = new BoxShapeDescription(new Vector3(0.5f));
        BoxShape shape = desc.Create();
        
        Entity mainCube = new Entity("MainCube");
        mainCube.AddComponent(new MeshRenderer(_mesh));
        mainCube.AddComponent(new Rigidbody(new BoxShapeDescription(new Vector3(0.5f)).Create(), 1));
        //model.AddToEntity(mainCube);
        AddEntity(mainCube);

        Entity secondCube = new Entity("Cube1", new Transform(new Vector3(0, -5, 0)));
        secondCube.AddComponent(new MeshRenderer(_mesh));
        secondCube.AddComponent(new Rigidbody(new BoxShapeDescription(new Vector3(0.5f)).Create(), 0));
        //mainCube.AddChild(secondCube);
        AddEntity(secondCube);

        Entity rayCube = new Entity("RayCube", new Transform() { Scale = new Vector3(0.1f) });
        rayCube.AddComponent(new MeshRenderer(_mesh));
        AddEntity(rayCube);
        
        //AddEntity(mainCube);
        
        Camera.Transform.Position = new Vector3(0, 0, 3);
        Camera.AddComponent(new CameraMove());
        
        /*Camera.Skybox = new Skybox(
            new Bitmap("/home/aqua/Pictures/skybox/spacebox/nizzine/right.png"),
            new Bitmap("/home/aqua/Pictures/skybox/spacebox/nizzine/left.png"),
            new Bitmap("/home/aqua/Pictures/skybox/spacebox/nizzine/top.png"),
            new Bitmap("/home/aqua/Pictures/skybox/spacebox/nizzine/bottom.png"),
            new Bitmap("/home/aqua/Pictures/skybox/spacebox/nizzine/front.png"),
            new Bitmap("/home/aqua/Pictures/skybox/spacebox/nizzine/back.png"));*/
        Camera.Skybox = Content.Content.Load<Skybox>("/home/aqua/Pictures/skybox/spacebox/nizzine/");
        
        Console.WriteLine(Matrix<float>.Identity);
        
        Console.WriteLine(Matrix<double>.Identity[3][3]);
        
        base.Initialize();
    }

    public override void Update(float dt)
    {
        base.Update(dt);
        
        if (Input.Input.IsKeyPressed(Key.Escape))
            App.Close();

        if (Physics.Physics.Raycast(Camera.Transform.Position, Camera.Transform.Forward, 100, out RaycastHit hit))
        {
            Entity rayCube = GetEntity("RayCube");
            //rayCube.Transform.Position = hit.BodyPosition + hit.SurfaceNormal;
            rayCube.Transform.Position = hit.WorldPosition;
        }

        if (Input.Input.IsMouseButtonPressed(MouseButton.Left) || Input.Input.IsMouseButtonDown(MouseButton.Right))
        {
            Entity entity = new Entity(Random.Shared.NextInt64().ToString(),
                new Transform(Camera.Transform.Position + Camera.Transform.Forward * 6));
            
            entity.AddComponent(new MeshRenderer(_mesh));
            
            AddEntity(entity);
        }

        /*GetEntity("MainCube").Transform.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, dt);
        GetEntity("MainCube/Cube1").Transform.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitX, dt);*/
    }

    public override void Draw()
    {
        base.Draw();
        
        //App.Renderer.DrawImage(_texture2, Vector2.Zero);
        //App.Renderer.DrawImage(_texture, Vector2.Zero);
        //App.Renderer.DrawImage(_texture, new Vector2(100));
        
        Renderer.DrawLine(new Vector2T<int>(0, 0), new Vector2T<int>(1280, 720), Color.White, 5);
    }
}