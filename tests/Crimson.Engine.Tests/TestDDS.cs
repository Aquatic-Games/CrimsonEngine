using Crimson.Engine.Entities;
using Crimson.Graphics;
using Crimson.Math;

namespace Crimson.Engine.Tests;

public class TestDDS : Scene
{
    private Texture _texture;
    
    public override void Initialize()
    {
        DDS dds = new DDS("Content/awesomeface.dds");
        _texture = new Texture(dds);
    }

    public override void Draw()
    {
        Renderer.DrawImage(_texture, Vector2T<int>.Zero);
    }

    public override void Dispose()
    {
        base.Dispose();
        
        _texture.Dispose();
    }
}