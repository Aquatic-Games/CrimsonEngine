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
    private List<Texture> _textures;

    public override async Task Load()
    {
        _textures = [];
        
        foreach (string path in Directory.GetFiles("/Users/aqua/Pictures"))
            _textures.Add(await Asset.LoadAsync<Texture>(path));
    }

    public override void Init()
    {
        base.Init();
        Renderer.BackgroundColor = Color.Crimson;
        _textures = [];

        _task = Load();
    }

    public override void Loop(float dt)
    {
        base.Loop(dt);

        if (!_task.IsCompleted)
            return;

        Renderer.DrawImage(_textures[0], new Vector2(0, 0));
        Console.WriteLine("Draw!");
    }
}