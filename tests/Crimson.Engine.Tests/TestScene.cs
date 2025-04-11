using Crimson.Engine.Entities;

namespace Crimson.Engine.Tests;

public class TestScene : Scene
{
    public override void Initialize()
    {
        Entity test = new Entity("test");
        test.AddComponent(new TestComponent());
        AddEntity(test);
        
        base.Initialize();
    }
}