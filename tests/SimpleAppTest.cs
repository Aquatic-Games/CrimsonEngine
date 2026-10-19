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
    public override void Init()
    {
        Renderer.BackgroundColor = Color.Crimson;
        base.Init();
    }
}