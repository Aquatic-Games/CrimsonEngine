using Crimson.Graphics;
using Crimson.Math;

namespace Crimson.UI;

public struct Theme
{
    public Font Font;

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

    public Theme(Font font)
    {
        Font = font;
        BorderSize = 2;
        TextSize = 24;
        HeadingSize = 48;
        Padding = 12;
    }

    public static Theme Light(Font font) => new Theme(font)
    {
        TextColor = Color.White,
        ButtonColor = Color.Gray,
        ButtonHoveredColor = Color.DarkGray,
        ButtonClickedColor = Color.Gray,
        ButtonBorderColor = Color.White,
        CheckboxSelectedColor = Color.LightGray
    };
}