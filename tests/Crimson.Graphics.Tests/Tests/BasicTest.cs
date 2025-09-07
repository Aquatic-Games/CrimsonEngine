namespace Crimson.Graphics.Tests.Tests;

public sealed class BasicTest() : TestBase("Basic Test")
{
    private Texture _texture = null!;

    protected override void Initialize()
    {
        _texture = new Texture("/home/aqua/Pictures/awesomeface.png");
    }

    public override void Dispose()
    {
        _texture.Dispose();
        
        base.Dispose();
    }
}