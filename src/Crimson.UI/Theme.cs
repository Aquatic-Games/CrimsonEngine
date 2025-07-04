using Crimson.Graphics;
using Crimson.Math;

namespace Crimson.UI;

public struct Theme
{
    public Font Font;

    public Color TextColor;

    public uint TextSize;
    
    public uint HeadingSize;

    public Color ButtonColor;

    public Color ButtonHoveredColor;

    public Color ButtonClickedColor;

    public Color ButtonBorderColor;

    public Theme(Font font)
    {
        Font = font;
        TextSize = 24;
        HeadingSize = 48;
    }

    public static Theme Light(Font font) => new Theme(font)
    {
        TextColor = Color.White,
        ButtonColor = Color.Gray,
        ButtonHoveredColor = Color.DarkGray,
        ButtonClickedColor = Color.Gray,
        ButtonBorderColor = Color.White
    };
}