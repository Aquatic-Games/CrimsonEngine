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
        _logo = new Texture(App.Renderer, "/home/aqua/Pictures/Aquatic Games/nizzine-aquaticgameswide.png");
        _logo2 = new Texture(App.Renderer, "/home/aqua/Pictures/Aquatic Games/nizzine-aquaticgameswide-night.png");
    }

    public override void Update(float dt)
    {
        if (_timer >= 7.0)
            App.Close();
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
        Renderer renderer = App.Renderer;

        renderer.DrawImage(_logo, Vector2.Zero, renderer.RenderSize, Color.White with { A = _logoAlpha });
        renderer.DrawImage(_logo2, Vector2.Zero, renderer.RenderSize, Color.White with { A = _logo2Alpha });
    }
}