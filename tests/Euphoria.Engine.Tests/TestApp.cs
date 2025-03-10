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

        //_texture = new Texture("/home/aqua/Pictures/awesomeface.png");
        _texture = new Texture(new Size<int>(1, 1), [255, 255, 255, 255]);
    }

    public override void Draw()
    {
        base.Draw();
        
        for (int y = 0; y < 720; y++)
            for (int x = 0; x < 1280; x++)
                Graphics.DrawImage(_texture, new Vector2(x, y));
    }

    public override void Dispose()
    {
        _texture.Dispose();
        
        base.Dispose();
    }
}