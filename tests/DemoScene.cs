#!/usr/bin/env dotnet
#:project ../src/Crimson.Engine/Crimson.Engine.csproj

using System.Numerics;
using Crimson.Engine;
using Crimson.Engine.Actors;
using Crimson.Engine.Actors.Components;
using Crimson.Graphics;
using Crimson.Input;

AppInfo info = new AppInfo("Demo Scene", "1.0.0");
App.Run(in info, new DemoScene());

class CameraMovement : Component
{
    private Vector2 _rotation;
    
    public override void Loop(float dt)
    {
        Vector2 mouseDelta = Input.MouseDelta;

        _rotation -= mouseDelta * 0.01f;
        _rotation.Y = float.Clamp(_rotation.Y, -float.Pi / 2, float.Pi / 2);

        Transform.Rotation = Quaternion.CreateFromYawPitchRoll(_rotation.X, _rotation.Y, 0);
    }
}

class DemoScene : Scene
{
    private Skybox _skybox = null!;
    
    public override void Init()
    {
        _skybox = new Skybox("CommonContent/Skybox/right.jpg", "CommonContent/Skybox/left.jpg", "CommonContent/Skybox/top.jpg",
            "CommonContent/Skybox/bottom.jpg", "CommonContent/Skybox/front.jpg", "CommonContent/Skybox/back.jpg");
        
        Camera.AddComponent(new CameraMovement());
        Camera.Skybox = _skybox;
        
        base.Init();
    }

    public override void Dispose()
    {
        base.Dispose();
        
        _skybox.Dispose();
    }
}