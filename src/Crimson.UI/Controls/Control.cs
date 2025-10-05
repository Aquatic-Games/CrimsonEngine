using Crimson.Math;
using Crimson.Platform;

namespace Crimson.UI.Controls;

public abstract class Control
{
    private Size<int> _size;
    
    protected float Scale;
    protected Rectangle<int> ScreenRegion;
    
    protected bool IsHovered;
    protected bool IsHeld;
    protected bool IsClicked;

    public Theme Theme = UI.Theme;

    public bool Disabled;

    public Size<int> Size
    {
        get => _size;
        set
        {
            _size = value;
            ScreenRegion.Size = (_size.As<float>() * Scale).As<int>();
        }
    }
    
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
            }
        }
        else
        {
            IsHovered = false;
            IsHeld = false;
        }
    }

    protected internal abstract void Draw();

    protected internal virtual void CalculateLayout(Vector2T<int> position, float scale)
    {
        Scale = scale;
        ScreenRegion.Position = position;
    }
}