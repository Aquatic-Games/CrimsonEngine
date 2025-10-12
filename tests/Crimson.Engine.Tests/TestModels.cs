using Crimson.Engine.Entities;
using Crimson.Graphics.Models.Loaders.GLTF;

namespace Crimson.Engine.Tests;

public class TestModels : Scene
{
    public override void Initialize()
    {
        Gltf gltf = Gltf.FromPath("/home/aqua/Downloads/Box.glb");
        Console.WriteLine(gltf.ToString());
        
        base.Initialize();
    }
}