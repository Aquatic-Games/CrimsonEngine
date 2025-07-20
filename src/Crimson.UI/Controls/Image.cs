using Crimson.Graphics;
using Crimson.Math;

namespace Crimson.UI.Controls;

public class Image : Control
{
    public Texture Texture;

    public Color Tint;
    
    public Image(Texture texture)
    {
        Texture = texture;
        Tint = Color.White;
    }
    
    protected internal override void Draw()
    {
        Renderer.DrawImage(Texture, ScreenRegion.Position, ScreenRegion.Size, tint: Tint);
    }
}