using Crimson.Graphics;
using Crimson.Math;

namespace Crimson.UI.Controls;

public class Checkbox : Control
{
    private string _text;
    
    public bool Checked;

    public Action<bool>? OnChecked;
    
    public string Text
    {
        get => _text;
        set => _text = Locale.GetLocalizedString(value);
    }

    public Checkbox(string text, Action<bool>? onChecked = null)
    {
        Text = text;
        Checked = false;
        OnChecked = onChecked;
    }

    protected internal override void Update(float dt, ref bool mouseCaptured, Vector2T<int> mousePos)
    {
        base.Update(dt, ref mouseCaptured, mousePos);

        if (IsClicked)
        {
            Checked = !Checked;
            OnChecked?.Invoke(Checked);
        }
    }

    protected internal override void Draw()
    {
        Theme theme = Theme.AsScaled(Scale);
        
        Color color;

        if (IsClicked)
            color = theme.ButtonClickedColor;
        else if (IsHovered)
            color = theme.ButtonHoveredColor;
        else
            color = theme.ButtonColor;

        Renderer.DrawRectangle(ScreenRegion.Position, ScreenRegion.Size, color, theme.BorderSize,
            theme.ButtonBorderColor);

        int halfPadding = theme.Padding / 2;

        if (Checked)
        {
            Renderer.DrawFilledRectangle(ScreenRegion.Position + new Vector2T<int>(halfPadding),
                ScreenRegion.Size - new Size<int>(halfPadding * 2), theme.CheckboxSelectedColor);
        }

        Size<int> textSize = theme.Font.MeasureText(_text, theme.TextSize);

        Renderer.DrawText(theme.Font,
            ScreenRegion.Position + new Vector2T<int>(ScreenRegion.Width + halfPadding,
                ScreenRegion.Height / 2 - textSize.Height / 2), theme.TextSize, _text, theme.TextColor);
    }
}