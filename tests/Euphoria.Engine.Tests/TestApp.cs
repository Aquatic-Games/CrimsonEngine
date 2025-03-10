using System.Numerics;
using Euphoria.Math;
using Euphoria.Render;

namespace Euphoria.Engine.Tests;

public class TestApp : GlobalApp
{
    private Texture _texture = null!;
    
    public override void Initialize()
    {
        base.Initialize();

        _texture = new Texture("/home/aqua/Pictures/awesomeface.png");
    }

    public override void Draw()
    {
        base.Draw();
        
        for (int y = 0; y < 100; y++)
            for (int x = 0; x < 50; x++)
                Graphics.DrawImage(_texture, new Vector2(x, y));
    }

    public override void Dispose()
    {
        _texture.Dispose();
        
        base.Dispose();
    }
}