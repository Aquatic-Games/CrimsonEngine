#!/usr/bin/env dotnet
#:project ../src/Crimson.Engine/Crimson.Engine.csproj

using Crimson.Engine;
using Crimson.Engine.Actors;
using Crimson.Engine.Actors.Components;
using Crimson.Input;
using Crimson.Platform;

// Tests components.
// There are two actors and two components.
// Test1 has both components.
// Test2 only has one component.
// Press the space bar to add the second component to Test2.
// Press it again to ensure the debug assertion works (in debug mode).
// This tests adding components both before and after initialization.

AppInfo info = new AppInfo("Components Test", "1.0.0");
App.Run(in info, new ComponentsTestScene());

class Component1 : Component
{
    public override void Init()
    {
        Console.WriteLine($"{Actor.Name}: Component 1 init");
    }

    public override void Loop(float dt)
    {
        Console.WriteLine($"{Actor.Name}: Component 1!");
    }
}

class Component2 : Component
{
    public override void Init()
    {
        Console.WriteLine($"{Actor.Name}: Component 2 init");
    }

    public override void Loop(float dt)
    {
        Console.WriteLine($"{Actor.Name}: Component 2!");
    }
}

class ComponentsTestScene : Scene
{
    public override void Init()
    {
        Actor test1 = new Actor("Test1")
            .AddComponent(new Component1())
            .AddComponent(new Component2());
        AddActor(test1);
        
        Actor test2 = new Actor("Test2")
            .AddComponent(new Component1());
        AddActor(test2);
        
        base.Init();
    }

    public override void Loop(float dt)
    {
        if (Input.IsKeyPressed(Key.Space))
            GetActor("Test2").AddComponent(new Component2());
        
        base.Loop(dt);
    }
}