using System.Numerics;
using Crimson.Audio;
using Crimson.Data;
using Crimson.Engine.Entities;
using Crimson.Engine.Entities.Components;
using Crimson.Graphics;
using Crimson.Graphics.Materials;
//using Crimson.Graphics.Materials;
using Crimson.Graphics.Primitives;
using Crimson.Input;
using Crimson.Input.Bindings;
using Crimson.Math;
using Crimson.Physics;
using Crimson.Physics.Shapes;
using Crimson.Physics.Shapes.Descriptions;
using Crimson.Platform;
using Crimson.UI.Controls;
using Crimson.UI.Controls.Layouts;
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

        ActionSet set = new ActionSet("Main", false);
        set.AddAction(new InputAction("Move", new DualAxisBinding(new KeyBinding(Key.W), new KeyBinding(Key.S), new KeyBinding(Key.D), new KeyBinding(Key.A))));
        set.AddAction(new InputAction("Look", new MouseMoveBinding(), new DualAxisBinding(new KeyBinding(Key.Up), new KeyBinding(Key.Down), new KeyBinding(Key.Right), new KeyBinding(Key.Left))));

        ActionSet otherSet = new ActionSet("UI", true);

        Input.Input.AddActionSet(set);
        Input.Input.AddActionSet(otherSet);
        Input.Input.SetCurrentActionSet(set.Name);
        
        _texture = Content.Content.Load<Texture>("DEBUG");

        //StreamSound sound = new StreamSound("/home/aqua/Music/kf-battle.ogg");
        //sound.Play();
        
        MaterialDefinition def = new(_texture)
        {
            RenderFace = RenderFace.Front
        };
        
        _material = new StandardLit(in def);
        _mesh = Mesh.FromPrimitive(new Cube(), _material);
        
        //Model model = Model.FromGltf("/home/aqua/Downloads/Fox.glb");

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
        
        AnchorLayout layout = (AnchorLayout) UI.UI.BaseControl;
        layout.Add(Anchor.TopLeft, Vector2T<int>.Zero, new Size<int>(200, 100), new Button("Top Left"));
        layout.Add(Anchor.TopMiddle, Vector2T<int>.Zero, new Size<int>(200, 100), new Button("Top Middle"));
        layout.Add(Anchor.TopRight, Vector2T<int>.Zero, new Size<int>(200, 100), new Button("Top Right"));
        layout.Add(Anchor.CenterLeft, Vector2T<int>.Zero, new Size<int>(200, 100), new Button("Center Left"));
        layout.Add(Anchor.CenterMiddle, Vector2T<int>.Zero, new Size<int>(200, 100), new Button("Center Middle"));
        layout.Add(Anchor.CenterRight, Vector2T<int>.Zero, new Size<int>(200, 100), new Button("Center Right"));
        layout.Add(Anchor.BottomLeft, Vector2T<int>.Zero, new Size<int>(200, 100), new Button("Bottom Left"));
        layout.Add(Anchor.BottomMiddle, Vector2T<int>.Zero, new Size<int>(200, 100), new Button("Bottom Middle"));
        layout.Add(Anchor.BottomRight, Vector2T<int>.Zero, new Size<int>(200, 100), new Button("Bottom Right"));

        layout.Add(Anchor.CenterMiddle, new Vector2T<int>(50), new Size<int>(100),
            new Button("hello", () => Console.WriteLine("hi!"))
                { Theme = UI.UI.Theme with { ButtonColor = Color.Brown } });
        
        base.Initialize();
    }

    public override void Update(float dt)
    {
        base.Update(dt);
        
        if (Input.Input.IsKeyPressed(Key.Escape))
            App.Close();
        
        if (Input.Input.IsKeyPressed(Key.P))
            Input.Input.SetCurrentActionSet("UI");
        if (Input.Input.IsKeyPressed(Key.O))
            Input.Input.SetCurrentActionSet("Main");

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
        
        //Renderer.DrawLine(new Vector2T<int>(0, 0), new Vector2T<int>(1280, 720), Color.White, 5);
    }
}