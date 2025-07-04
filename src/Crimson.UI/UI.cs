using System.Numerics;
using Crimson.Math;

namespace Crimson.UI;

public static class UI
{
    public static Control BaseControl = null!;

    public static Rectangle<int> ScreenRegion;

    public static void Create(Rectangle<int> screenRegion)
    {
        ScreenRegion = screenRegion;
        
        BaseControl.CalculateLayout(ScreenRegion);
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
}