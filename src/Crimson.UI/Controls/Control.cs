using Crimson.Math;
using Crimson.Platform;

namespace Crimson.UI.Controls;

public abstract class Control
{
    protected Rectangle<int> ScreenRegion;

    protected bool IsHovered;
    protected bool IsHeld;
    protected bool IsClicked;
    
    protected internal virtual void Initialize() { }

    protected internal virtual void Update(float dt, ref bool mouseCaptured, Vector2T<int> mousePos)
    {
        IsClicked = false;
        
        if (!mouseCaptured && ScreenRegion.Contains(mousePos))
        {
            mouseCaptured = true;
            IsHovered = true;

            if (Input.Input.IsMouseButtonDown(MouseButton.Left))
                IsHeld = true;
            else if (IsHeld)
            {
                IsHeld = false;
                IsClicked = true;
            }
        }
        else
        {
            IsHovered = false;
            IsHeld = false;
        }
    }

    protected internal abstract void Draw();

    protected internal virtual void CalculateLayout(Rectangle<int> region)
    {
        ScreenRegion = region;
    }
}