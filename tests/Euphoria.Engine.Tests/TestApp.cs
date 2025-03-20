using System.Numerics;
using Euphoria.Math;
using Euphoria.Render;

namespace Euphoria.Engine.Tests;

public class TestApp : GlobalApp
{
    private Texture _texture = null!;
    private Texture _texture2 = null!;
    
    public override void Initialize()
    {
        base.Initialize();

        _texture = new Texture("/home/aqua/Pictures/awesomeface.png");
        _texture2 = new Texture("/home/aqua/Pictures/BAGELMIP.png");
    }

    public override void Draw()
    {
        base.Draw();
        
        Graphics.DrawImage(_texture, new Vector2(0, 0));
        Graphics.DrawImage(_texture2, new Vector2(0, 0));
    }

    public override void Dispose()
    {
        _texture2.Dispose();
        _texture.Dispose();
        
        base.Dispose();
    }
}