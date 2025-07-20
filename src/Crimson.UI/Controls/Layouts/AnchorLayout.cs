using Crimson.Math;

namespace Crimson.UI.Controls.Layouts;

public class AnchorLayout : Control
{
    private readonly List<AnchorControl> _controls;
    private bool _layoutDirty;

    public AnchorLayout()
    {
        _controls = [];
    }
    
    public void Add(Anchor anchor, Vector2T<int> offset, Size<int> size, Control control)
    {
        _controls.Add(new AnchorControl(anchor, offset, size, control));
        _layoutDirty = true;
    }

    protected internal override void Update(float dt, ref bool mouseCaptured, Vector2T<int> mousePos)
    {
        // Only recalculate the layout on change.
        if (_layoutDirty)
            CalculateLayout(ScreenRegion, Scale);
        
        for (int i = _controls.Count - 1; i >= 0; i--)
            _controls[i].Control.Update(dt, ref mouseCaptured, mousePos);
    }

    protected internal override void Draw()
    {
        foreach (AnchorControl control in _controls)
            control.Control.Draw();
    }

    protected internal override void CalculateLayout(Rectangle<int> region, float scale)
    {
        base.CalculateLayout(region, scale);
        
        _layoutDirty = false;
        
        foreach (AnchorControl control in _controls)
        {
            Vector2T<int> offset = (control.Offset.As<float>() * scale).As<int>();
            Size<int> layoutSize = ScreenRegion.Size;
            Size<int> size = (control.Size.As<float>() * scale).As<int>();
            
            offset += control.Anchor switch
            {
                Anchor.TopLeft => Vector2T<int>.Zero,
                Anchor.TopMiddle => new Vector2T<int>(layoutSize.Width / 2 - size.Width / 2, 0),
                Anchor.TopRight => new Vector2T<int>(layoutSize.Width - size.Width, 0),
                Anchor.CenterLeft => new Vector2T<int>(0, layoutSize.Height/ 2 - size.Height / 2),
                Anchor.CenterMiddle => new Vector2T<int>(layoutSize.Width / 2 - size.Width / 2, layoutSize.Height / 2 - size.Height / 2),
                Anchor.CenterRight => new Vector2T<int>(layoutSize.Width - size.Width, layoutSize.Height / 2 - size.Height / 2),
                Anchor.BottomLeft => new Vector2T<int>(0, layoutSize.Height - size.Height),
                Anchor.BottomMiddle => new Vector2T<int>(layoutSize.Width / 2 - size.Width / 2, layoutSize.Height - size.Height),
                Anchor.BottomRight => new Vector2T<int>(layoutSize.Width - size.Width, layoutSize.Height - size.Height),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            control.Control.CalculateLayout(new Rectangle<int>(offset, size), scale);
        }
    }

    private readonly struct AnchorControl
    {
        public readonly Anchor Anchor;
        public readonly Vector2T<int> Offset;
        public readonly Size<int> Size;
        public readonly Control Control;

        public AnchorControl(Anchor anchor, Vector2T<int> offset, Size<int> size, Control control)
        {
            Anchor = anchor;
            Offset = offset;
            Size = size;
            Control = control;
        }
    }
}