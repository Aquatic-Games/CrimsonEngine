using Crimson.Graphics;

namespace Crimson.UI.Controls;

public class Image : Control
{
    public Texture Texture;
    
    public Image(Texture texture)
    {
        Texture = texture;
    }
    
    protected internal override void Draw()
    {
        Renderer.DrawImage(Texture, ScreenRegion.Position, ScreenRegion.Size);
    }
}