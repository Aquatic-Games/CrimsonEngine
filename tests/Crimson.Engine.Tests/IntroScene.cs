using System.Numerics;
using Crimson.Engine.Entities;
using Crimson.Graphics;
using Crimson.Math;

namespace Crimson.Engine.Tests;

public class IntroScene : Scene
{
    private Texture _logo;
    private Texture _logo2;

    private float _logoAlpha;
    private float _logo2Alpha;

    private double _timer;
    
    public override void Initialize()
    {
        _logo = Content.Content.Load<Texture>("nizzine-aquaticgameswide");
        _logo2 = Content.Content.Load<Texture>("nizzine-aquaticgameswide-night");
    }

    public override void Update(float dt)
    {
        if (_timer >= 7.0)
            App.SetScene(new TestScene());
        else if (_timer >= 5.0)
        {
            _logoAlpha = 0;
            _logo2Alpha -= dt;
            _logo2Alpha = float.Clamp(_logo2Alpha, 0, 1);
        }
        else if (_timer >= 3.0)
        {
            _logo2Alpha += dt;
            _logo2Alpha = float.Clamp(_logo2Alpha, 0, 1);
        }
        else if (_timer >= 1.0)
        {
            _logoAlpha += dt;
            _logoAlpha = float.Clamp(_logoAlpha, 0, 1);
        }

        _timer += dt;
    }

    public override void Draw()
    {
        Renderer.DrawImage(_logo, Vector2T<int>.Zero, Renderer.RenderSize, tint: Color.White with { A = _logoAlpha });
        Renderer.DrawImage(_logo2, Vector2T<int>.Zero, Renderer.RenderSize, tint: Color.White with { A = _logo2Alpha });
    }
}