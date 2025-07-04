using System.Numerics;
using Crimson.Graphics;
using Crimson.Math;
using Crimson.UI.Controls;
using Crimson.UI.Controls.Layouts;

namespace Crimson.UI;

public static class UI
{
    private static Rectangle<int> _screenRegion;
    
    public static Control BaseControl = null!;

    public static Theme Theme;

    public static void Create(Rectangle<int> screenRegion)
    {
        _screenRegion = screenRegion;
        // TODO: Obviously this only works on my machine.
        Theme = Theme.Light(new Font("/home/aqua/Documents/Roboto-Regular.ttf"));

        BaseControl = new AnchorLayout();
        BaseControl.CalculateLayout(_screenRegion);
    }

    public static void Update(float dt)
    {
        Vector2 mPos = Input.Input.MousePosition;
        Vector2T<int> mousePos = new Vector2T<int>((int) mPos.X, (int) mPos.Y);
        bool mouseCaptured = false;
        BaseControl.Update(dt, ref mouseCaptured, mousePos);
    }

    public static void Draw()
    {
        BaseControl.Draw();
    }

    public static void Resize(Rectangle<int> screenRegion)
    {
        _screenRegion = screenRegion;
        BaseControl.CalculateLayout(_screenRegion);
    }
}