using Crimson.Engine.Entities;
using Crimson.Graphics;
using Crimson.Math;

namespace Crimson.Engine.Tests;

public class TestDDS : Scene
{
    private Texture _texture;
    private Texture _texture2;
    
    public override void Initialize()
    {
        _texture = Content.Content.Load<Texture>("24bitcolor-BC7");
        _texture2 = Content.Content.Load<Texture>("DEBUG");
    }

    public override void Draw()
    {
        Renderer.DrawImage(_texture, Vector2T<int>.Zero);
        Renderer.DrawImage(_texture2, Vector2T<int>.Zero);
    }
}