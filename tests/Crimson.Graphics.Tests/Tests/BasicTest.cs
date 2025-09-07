using Crimson.Math;

namespace Crimson.Graphics.Tests.Tests;

public sealed class BasicTest() : TestBase("Basic Test")
{
    private Texture _texture = null!;
    private Font _font = null!;

    protected override void Initialize()
    {
        _texture = new Texture("Content/awesomeface.png");
        _font = new Font("Content/NotoSansJP-Regular.ttf");
    }

    protected override void Draw()
    {
        for (int i = 0; i < 10; i++) 
            Renderer.DrawImage(_texture, new Vector2T<int>(i * 50), _texture.Size / 2);
        
        Renderer.DrawText(_font, new Vector2T<int>(700, 100), 48, "It works!", Color.White);
    }

    public override void Dispose()
    {
        _font.Dispose();
        _texture.Dispose();
        
        base.Dispose();
    }
}