using Euphoria.Render;

namespace Euphoria.Engine.Tests;

public class TestApp : GlobalApp
{
    private Texture _texture = null!;
    
    public override void Initialize()
    {
        base.Initialize();

        _texture = new Texture("/home/aqua/Pictures/awesomeface.png");
    }

    public override void Dispose()
    {
        _texture.Dispose();
        
        base.Dispose();
    }
}