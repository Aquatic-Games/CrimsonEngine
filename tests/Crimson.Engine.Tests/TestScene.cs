using Crimson.Engine.Entities;
using Crimson.Engine.Entities.Components;
using Crimson.Render;
using Crimson.Render.Primitives;

namespace Crimson.Engine.Tests;

public class TestScene : Scene
{
    public override void Initialize()
    {
        Material material = new Material(App.Graphics,
            new MaterialDefinition(new Texture(App.Graphics, "C:/Users/aqua/Pictures/DEBUG.png")));
        
        Entity test = new Entity("test");
        test.AddComponent(new MeshRenderer(Mesh.FromPrimitive(new Cube(), material)));
        test.AddComponent(new TestComponent());
        AddEntity(test);
        
        base.Initialize();
    }
}