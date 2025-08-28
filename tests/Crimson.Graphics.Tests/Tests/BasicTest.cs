namespace Crimson.Graphics.Tests.Tests;

public sealed class BasicTest : TestBase
{
    private Texture _texture;

    protected override void Initialize()
    {
        _texture = new Texture(Bitmap.Debug);
    }

    public override void Dispose()
    {
        _texture.Dispose();
    }

    public BasicTest() : base("Basic Test") { }
}