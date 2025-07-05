using System.Numerics;
using System.Reflection;
using Crimson.Content;
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

    public static void Create(Rectangle<int> screenRegion, UIOptions options)
    {
        _screenRegion = screenRegion;
        
        Theme = Theme.Light;
        // The content manager doesn't have the ability to load persistent resources yet so we have to manually do what
        // the content manager does.
        Theme.Font = options.DefaultFont != null
            ? Font.LoadResource(Content.Content.GetFullyQualifiedName(options.DefaultFont), Path.HasExtension(options.DefaultFont))
            : new Font(Resources.LoadEmbeddedResource("Crimson.UI.Roboto-Regular.ttf",
                Assembly.GetExecutingAssembly()));

        BaseControl = new AnchorLayout();
        BaseControl.CalculateLayout(_screenRegion);
    }

    public static void Destroy()
    {
        Theme.Font.Dispose();
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