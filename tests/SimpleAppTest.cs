#!/usr/bin/env dotnet
#:project ../src/Crimson.Engine/Crimson.Engine.csproj

using System.Numerics;
using Crimson.Assets;
using Crimson.Engine;
using Crimson.Engine.Actors;
using Crimson.Graphics;

AppInfo appInfo = new("Simple App Test", "1.0.0");
App.Run(in appInfo, new TestApp());

class TestApp : Scene
{
    private Task _task;
    private Texture _texture1;
    private float _value;
    
    private List<Texture> _textures;

    public override void Load()
    {
        _textures = [];

        foreach (string path in Directory.EnumerateFiles("/home/aqua/Pictures", "*.png"))
            _textures.Add(Asset.Load<Texture>(path));
    }

    public override void Init()
    {
        _texture1 = new Texture("CommonContent/DEBUG.png");
        
        _task = Task.Run(Load);
        Renderer.BackgroundColor = Color.Crimson;
        
        base.Init();
    }

    public override void Loop(float dt)
    {
        base.Loop(dt);

        _value += dt;
        if (_value >= float.Pi * 2)
            _value -= float.Pi * 2;
        
        Renderer.DrawImage(_texture1, Matrix3x2.CreateRotation(_value) * Matrix3x2.CreateTranslation(100, 100));

        if (!_task.IsCompleted)
            return;

        Renderer.DrawImage(_textures[0], new Vector2(0, 0));
        Console.WriteLine("Draw!");
    }
}