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
        _texture = Content.Content.Load<Texture>("DEBUG");
        _texture = Content.Content.Load<Texture>("DEBUG");
        _font = Content.Content.Load<Font>("/home/aqua/Documents/Roboto-Regular");
        
        Camera.Type = CameraType.Orthographic;
        
        /*Texture spriteTexture = Content.Content.Load<Texture>("/home/aqua/Pictures/BAGELMIP.png");
        Entity entity = new Entity("Sprite");
        entity.AddComponent(new Sprite(spriteTexture));
        entity.AddComponent(new Move2D());
        AddEntity(entity);*/

        AnchorLayout layout = (AnchorLayout) UI.UI.BaseControl;
        /*layout.Add(Anchor.TopLeft, Vector2T<int>.Zero, new Size<int>(200, 100), new Button("Top Left"));
        layout.Add(Anchor.TopMiddle, Vector2T<int>.Zero, new Size<int>(200, 100), new Button("Top Middle"));
        layout.Add(Anchor.TopRight, Vector2T<int>.Zero, new Size<int>(200, 100), new Button("Top Right"));
        layout.Add(Anchor.CenterLeft, Vector2T<int>.Zero, new Size<int>(200, 100), new Button("Center Left"));
        layout.Add(Anchor.CenterMiddle, Vector2T<int>.Zero, new Size<int>(200, 100), new Button("Center Middle"));
        layout.Add(Anchor.CenterRight, Vector2T<int>.Zero, new Size<int>(200, 100), new Button("Center Right"));
        layout.Add(Anchor.BottomLeft, Vector2T<int>.Zero, new Size<int>(200, 100), new Button("Bottom Left"));
        layout.Add(Anchor.BottomMiddle, Vector2T<int>.Zero, new Size<int>(200, 100), new Button("Bottom Middle"));
        layout.Add(Anchor.BottomRight, Vector2T<int>.Zero, new Size<int>(200, 100), new Button("Bottom Right"));

        layout.Add(Anchor.CenterMiddle, new Vector2T<int>(50), new Size<int>(100),
            new Button("hello", () => Console.WriteLine("hi!"))
                { Theme = UI.UI.Theme with { ButtonColor = Color.Brown } });*/

        layout.Add(Anchor.TopLeft, new Vector2T<int>(10), new Size<int>(30),
            new Checkbox("Checkbox", b => Console.WriteLine(b)));
        
        base.Initialize();
    }

    public override void Draw()
    {
        base.Draw();

        // Check baseline alignment and size calculation with various letters
        const string text = "Hello World!\nHello Everything!\nqtLpa";
        const uint size = 48;
        
        Renderer.DrawFilledRectangle(Vector2T<int>.Zero, _font.MeasureText(text, size), Color.Orange);
        
        Renderer.DrawText(_font, new Vector2T<int>(0), size, text, Color.White);
        
        Renderer.DrawImage(_texture, Vector2T<int>.Zero, new Rectangle<int>(128, 0, 128, 64));

        //Renderer.DrawRectangle(new Vector2T<int>(10), new Size<int>(200, 100), Color.Orange, 5, Color.White);
    }
}