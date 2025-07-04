using Crimson.Math;

namespace Crimson.UI;

public abstract class Control
{
    protected Rectangle<int> ScreenRegion;
    
    protected internal virtual void Initialize() { }

    protected internal virtual void Update(float dt, ref bool mouseCaptured, Vector2T<int> mousePos)
    {
        if (!mouseCaptured && ScreenRegion.Contains(mousePos))
        {
            mouseCaptured = true;
        }
    }

    protected internal abstract void Draw();

    protected internal virtual void CalculateLayout(Rectangle<int> region)
    {
        ScreenRegion = region;
    }
}