using Crimson.Engine.Entities;
using Crimson.Graphics;
using Crimson.Math;
using Sprite = Crimson.Engine.Entities.Components.Sprite;

namespace Crimson.Engine.Tests;

public class Test2D : Scene
{
    private Texture _texture = null!;
    private Font _font = null!;
    
    public override void Initialize()
    {
        // TODO: Texture.Debug?
        _texture = Content.Content.Load<Texture>("/home/aqua/Pictures/DEBUG.png");

        _font = new Font("/home/aqua/Documents/Roboto-Regular.ttf");
        
        Camera.Type = CameraType.Orthographic;
        
        Texture spriteTexture = Content.Content.Load<Texture>("/home/aqua/Pictures/BAGELMIP.png");
        Entity entity = new Entity("Sprite");
        entity.AddComponent(new Sprite(spriteTexture));
        entity.AddComponent(new Move2D());
        AddEntity(entity);
        
        base.Initialize();
    }

    public override void Draw()
    {
        base.Draw();
        
        Renderer.DrawText(_font, new Vector2T<int>(100), 48, "Hello World!", Color.White);
        
        //Renderer.DrawImage(_texture, Vector2T<int>.Zero, new Rectangle<int>(128, 0, 128, 64));
    }
}