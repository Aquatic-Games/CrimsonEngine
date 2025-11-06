using Crimson.Math;
using Crimson.Platform;

namespace Crimson.UI.Controls;

public abstract class Control
{
    public Action? OnClicked;
    
    protected Rectangle<int> ScreenRegion;
    protected float Scale;

    protected bool IsHovered;
    protected bool IsHeld;
    protected bool IsClicked;

    public Theme Theme = UI.Theme;

    public bool Disabled;
    
    protected internal virtual void Initialize() { }

    protected internal virtual void Update(float dt, ref bool mouseCaptured, Vector2T<int> mousePos)
    {
        IsClicked = false;
        
        if (!mouseCaptured && !Disabled && ScreenRegion.Contains(mousePos))
        {
            mouseCaptured = true;
            IsHovered = true;

            if (Input.Input.IsMouseButtonDown(MouseButton.Left))
                IsHeld = true;
            else if (IsHeld)
            {
                IsHeld = false;
                IsClicked = true;
                OnClicked?.Invoke();
            }
        }
        else
        {
            IsHovered = false;
            IsHeld = false;
        }
    }

    protected internal abstract void Draw();

    protected internal virtual void CalculateLayout(Rectangle<int> region, float scale)
    {
        ScreenRegion = region;
        Scale = scale;
    }
}