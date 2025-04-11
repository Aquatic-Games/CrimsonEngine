using Crimson.Engine.Entities;

namespace Crimson.Engine.Tests;

public class TestComponent : Component
{
    public override void Initialize()
    {
        Console.WriteLine("Initialize");
    }

    public override void Update(float dt)
    {
        Console.WriteLine("Update");
    }

    public override void Draw()
    {
        Console.WriteLine("Draw");
    }

    public override void Dispose()
    {
        Console.WriteLine("Dispose");
    }
}