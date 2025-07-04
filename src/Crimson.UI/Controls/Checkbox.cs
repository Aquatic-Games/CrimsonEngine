using Crimson.Graphics;
using Crimson.Math;

namespace Crimson.UI.Controls;

public class Checkbox : Control
{
    public bool Checked;
    
    public string Text;

    public Action<bool>? OnChecked;

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
        Color color;

        if (IsClicked)
            color = Theme.ButtonClickedColor;
        else if (IsHovered)
            color = Theme.ButtonHoveredColor;
        else
            color = Theme.ButtonColor;

        Renderer.DrawRectangle(ScreenRegion.Position, ScreenRegion.Size, color, Theme.BorderSize,
            Theme.ButtonBorderColor);

        int halfPadding = Theme.Padding / 2;

        if (Checked)
        {
            Renderer.DrawFilledRectangle(ScreenRegion.Position + new Vector2T<int>(halfPadding),
                ScreenRegion.Size - new Size<int>(halfPadding * 2), Theme.CheckboxSelectedColor);
        }

        Size<int> textSize = Theme.Font.MeasureText(Text, Theme.TextSize);
        
        Renderer.DrawText(Theme.Font,
            ScreenRegion.Position + new Vector2T<int>(ScreenRegion.Width + halfPadding, ScreenRegion.Height / 2 - textSize.Height / 2), Theme.TextSize, Text,
            Theme.TextColor);
    }
}