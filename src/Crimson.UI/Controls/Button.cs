using Crimson.Graphics;
using Crimson.Math;

namespace Crimson.UI.Controls;

public class Button : Control
{
    protected internal override void Update(float dt, ref bool mouseCaptured, Vector2T<int> mousePos)
    {
        base.Update(dt, ref mouseCaptured, mousePos);
        
        if (IsClicked)
            Console.WriteLine("Click");
    }

    protected internal override void Draw()
    {
        Color color;
        
        if (IsHeld)
            color = Color.Gray;
        else if (IsHovered)
            color = Color.DarkGray;
        else
            color = Color.DarkSlateGray;
        
        Renderer.DrawRectangle(ScreenRegion.Position, ScreenRegion.Size, color, 2, Color.White);
    }
}