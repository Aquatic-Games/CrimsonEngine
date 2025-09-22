using Crimson.Graphics;
using Crimson.Math;

namespace Crimson.UI.Controls;

public class Button : Control
{
    private string _text;

    public Action? OnClicked;
    
    public string Text
    {
        get => _text;
        set => _text = Locale.GetLocalizedString(value);
    }

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
        Theme theme = Theme.AsScaled(Scale);
        
        Color color;
        
        if (IsHeld)
            color = theme.ButtonClickedColor;
        else if (IsHovered)
            color = theme.ButtonHoveredColor;
        else
            color = theme.ButtonColor;

        Renderer.DrawRectangle(ScreenRegion.Position, ScreenRegion.Size, color, theme.BorderSize,
            theme.ButtonBorderColor);

        Size<int> textSize = theme.Font.MeasureText(_text, theme.TextSize);
        Vector2T<int> textPos = ScreenRegion.Position + new Vector2T<int>(ScreenRegion.Width / 2 - textSize.Width / 2,
            ScreenRegion.Height / 2 - textSize.Height / 2);
        
        Renderer.DrawText(theme.Font, textPos, theme.TextSize, _text, theme.TextColor);
        
        if (Disabled)
            Renderer.DrawFilledRectangle(ScreenRegion.Position, ScreenRegion.Size, new Color(0, 0, 0, 0.5f));
    }
}