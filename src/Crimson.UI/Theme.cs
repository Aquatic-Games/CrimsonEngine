using Crimson.Graphics;
using Crimson.Math;

namespace Crimson.UI;

public struct Theme
{
    public Font Font = null!;

    public Color TextColor;

    public uint TextSize;
    
    public uint HeadingSize;

    public int Padding;

    public int BorderSize;

    public Color ButtonColor;

    public Color ButtonHoveredColor;

    public Color ButtonClickedColor;

    public Color ButtonBorderColor;

    public Color CheckboxSelectedColor;

    public Theme()
    {
        BorderSize = 2;
        TextSize = 24;
        HeadingSize = 48;
        Padding = 12;
    }

    public Theme AsScaled(float scale)
    {
        return this with
        {
            TextSize = (uint) (TextSize * scale),
            HeadingSize = (uint) (HeadingSize * scale),
            Padding = (int) (Padding * scale),
            BorderSize = int.Max((int) (BorderSize * scale), 1)
        };
    }

    public static Theme Light => new Theme
    {
        TextColor = Color.White,
        ButtonColor = Color.Gray,
        ButtonHoveredColor = Color.DarkGray,
        ButtonClickedColor = Color.Gray,
        ButtonBorderColor = Color.White,
        CheckboxSelectedColor = Color.LightGray
    };
}