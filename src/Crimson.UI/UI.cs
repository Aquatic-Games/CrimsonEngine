using System.Numerics;
using Crimson.Math;
using Crimson.UI.Controls;

namespace Crimson.UI;

public static class UI
{
    private static Rectangle<int> _screenRegion;
    
    public static Control BaseControl = null!;

    public static void Create(Rectangle<int> screenRegion)
    {
        _screenRegion = screenRegion;

        BaseControl = new Button();
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