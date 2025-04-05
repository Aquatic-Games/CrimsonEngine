using System.Numerics;
using Euphoria.Math;
using Euphoria.Render;

namespace Euphoria.Engine.Tests;

public class TestApp : GlobalApp
{
    private Texture _texture = null!;
    private Texture _texture2 = null!;

    private float _value = 0;
    
    public override void Initialize()
    {
        base.Initialize();

        _texture = new Texture("/home/aqua/Pictures/awesomeface.png");
        _texture2 = new Texture("/home/aqua/Pictures/BAGELMIP.png");
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        _value += dt;
        while (_value >= MathF.PI * 2)
            _value -= MathF.PI * 2;
    }

    public override void Draw()
    {
        base.Draw();
        
        /*Graphics.DrawImage(_texture, new Vector2(0, 0));
        Graphics.DrawImage(_texture2, new Vector2(float.Sin(_value) * 400 + 400, 0));*/
    }

    public override void Dispose()
    {
        _texture2.Dispose();
        _texture.Dispose();
        
        base.Dispose();
    }
}