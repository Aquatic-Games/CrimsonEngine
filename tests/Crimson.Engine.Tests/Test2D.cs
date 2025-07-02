using Crimson.Engine.Entities;
using Crimson.Graphics;
using Crimson.Math;
using Sprite = Crimson.Engine.Entities.Components.Sprite;

namespace Crimson.Engine.Tests;

public class Test2D : Scene
{
    public override void Initialize()
    {
        Camera.Type = CameraType.Orthographic;
        
        Texture spriteTexture = Content.Content.Load<Texture>("/home/aqua/Pictures/BAGELMIP.png");
        Entity entity = new Entity("Sprite");
        entity.AddComponent(new Sprite(spriteTexture));
        entity.AddComponent(new Move2D());
        AddEntity(entity);
        
        base.Initialize();
    }
}