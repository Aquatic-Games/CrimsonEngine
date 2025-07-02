using Crimson.Engine.Entities;
using Crimson.Graphics;
using Crimson.Math;
using Sprite = Crimson.Engine.Entities.Components.Sprite;

namespace Crimson.Engine.Tests;

public class Test2D : Scene
{
    private Texture _texture = null!;
    
    public override void Initialize()
    {
        // TODO: Texture.Debug?
        _texture = Content.Content.Load<Texture>("/home/aqua/Pictures/DEBUG.png");
        
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
        
        Renderer.DrawImage(_texture, Vector2T<int>.Zero, new Rectangle<int>(Vector2T<int>.Zero, Renderer.RenderSize));
    }
}