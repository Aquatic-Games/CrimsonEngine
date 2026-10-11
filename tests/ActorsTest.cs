#!/usr/bin/env dotnet
#:project ../src/Crimson.Engine/Crimson.Engine.csproj

using Crimson.Engine;
using Crimson.Engine.Actors;

AppInfo info = new AppInfo("Actors Test", "1.0.0");
App.Run(in info, new ActorsTestScene());

class MyActor(string name) : Actor(name)
{
    public override void Init()
    {
        base.Init();
        
        Console.WriteLine($"Initialize {Name}");
    }

    public override void Tick(float dt)
    {
        base.Tick(dt);
        
        Console.WriteLine($"Hello I'm actor {Name}!");
    }
}

class ActorsTestScene : Scene
{
    public override void Init()
    {
        Actor aqua = new MyActor("Aqua");
        AddActor(aqua);
        
        Actor lily = new MyActor("Lily");
        AddActor(lily);
        
        base.Init();
    }
}