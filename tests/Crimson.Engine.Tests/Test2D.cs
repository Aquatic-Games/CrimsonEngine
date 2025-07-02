using Crimson.Engine.Entities;
using Crimson.Graphics;
using Crimson.Math;

namespace Crimson.Engine.Tests;

public class Test2D : Scene
{
    private Texture _texture = null!;

    public override void Initialize()
    {
        Camera.Type = CameraType.Orthographic;
        
        _texture = new Texture("/home/aqua/Pictures/BAGELMIP.png");
        
        base.Initialize();
    }

    public override void Draw()
    {
        base.Draw();
        
        Renderer.DrawSprite(new Sprite(_texture), Matrix.RotateZ<float>(1));
        //Renderer.DrawImage(_texture, Vector2T<int>.Zero);
    }
}