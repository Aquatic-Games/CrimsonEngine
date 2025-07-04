using Crimson.Graphics;
using Crimson.Math;

namespace Crimson.UI.Controls;

public class Button : Control
{
    public string Text;

    public Action? OnClicked;

    public Button(string text, Action? onClicked = null)
    {
        Text = text;
        OnClicked = onClicked;
    }
    
    protected internal override void Update(float dt, ref bool mouseCaptured, Vector2T<int> mousePos)
    {
        base.Update(dt, ref mouseCaptured, mousePos);
        
        if (IsClicked)
            OnClicked?.Invoke();
    }

    protected internal override void Draw()
    {
        Color color;
        
        if (IsHeld)
            color = Theme.ButtonClickedColor;
        else if (IsHovered)
            color = Theme.ButtonHoveredColor;
        else
            color = Theme.ButtonColor;

        Renderer.DrawRectangle(ScreenRegion.Position, ScreenRegion.Size, color, Theme.BorderSize,
            Theme.ButtonBorderColor);

        Size<int> textSize = Theme.Font.MeasureText(Text, Theme.TextSize);
        Vector2T<int> textPos = ScreenRegion.Position + new Vector2T<int>(ScreenRegion.Width / 2 - textSize.Width / 2, ScreenRegion.Height / 2 - textSize.Height / 2);
        
        Renderer.DrawText(Theme.Font, textPos, Theme.TextSize, Text, Theme.TextColor);
    }
}