using Crimson.Engine.Entities;
using Crimson.Graphics;
using Crimson.Math;
using Crimson.UI.Controls;
using Crimson.UI.Controls.Layouts;
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
        _font = Content.Content.Load<Font>("/home/aqua/Documents/Roboto-Regular");
        
        Camera.Type = CameraType.Orthographic;
        
        Texture spriteTexture = Content.Content.Load<Texture>("/home/aqua/Pictures/BAGELMIP.png");
        Entity entity = new Entity("Sprite");
        entity.AddComponent(new Sprite(spriteTexture));
        entity.AddComponent(new Move2D());
        AddEntity(entity);

        AnchorLayout layout = (AnchorLayout) UI.UI.BaseControl;
        layout.Add(Anchor.TopLeft, Vector2T<int>.Zero, new Size<int>(100, 100), new Button());
        layout.Add(Anchor.TopMiddle, Vector2T<int>.Zero, new Size<int>(100, 100), new Button());
        layout.Add(Anchor.TopRight, Vector2T<int>.Zero, new Size<int>(100, 100), new Button());
        layout.Add(Anchor.CenterLeft, Vector2T<int>.Zero, new Size<int>(100, 100), new Button());
        layout.Add(Anchor.CenterMiddle, Vector2T<int>.Zero, new Size<int>(100, 100), new Button());
        layout.Add(Anchor.CenterRight, Vector2T<int>.Zero, new Size<int>(100, 100), new Button());
        layout.Add(Anchor.BottomLeft, Vector2T<int>.Zero, new Size<int>(100, 100), new Button());
        layout.Add(Anchor.BottomMiddle, Vector2T<int>.Zero, new Size<int>(100, 100), new Button());
        layout.Add(Anchor.BottomRight, Vector2T<int>.Zero, new Size<int>(100, 100), new Button());

        layout.Add(Anchor.CenterMiddle, new Vector2T<int>(50), new Size<int>(100), new Button());
        
        base.Initialize();
    }

    public override void Draw()
    {
        base.Draw();
        
        Renderer.DrawText(_font, new Vector2T<int>(100), 48, "Hello World!", Color.White);
        
        //Renderer.DrawImage(_texture, Vector2T<int>.Zero, new Rectangle<int>(128, 0, 128, 64));

        Renderer.DrawRectangle(new Vector2T<int>(10), new Size<int>(200, 100), Color.Orange, 5, Color.White);
    }
}