using Crimson.Math;

namespace Crimson.UI.Controls.Layouts;

public class AnchorLayout : Control
{
    private bool _layoutDirty;
    
    public readonly List<AnchorControl> Controls;

    public AnchorLayout()
    {
        Controls = [];
        _layoutDirty = true;
    }
    
    public void Add(Anchor anchor, Vector2T<int> offset, Control control)
    {
        Controls.Add(new AnchorControl(anchor, offset, control));
        _layoutDirty = true;
    }

    protected internal override void Update(float dt, ref bool mouseCaptured, Vector2T<int> mousePos)
    {
        // Only recalculate the layout on change.
        if (_layoutDirty)
            CalculateLayout(ScreenRegion.Position, Scale);
        
        for (int i = Controls.Count - 1; i >= 0; i--)
            Controls[i].Control.Update(dt, ref mouseCaptured, mousePos);
    }

    protected internal override void Draw()
    {
        foreach (AnchorControl control in Controls)
            control.Control.Draw();
    }

    protected internal override void CalculateLayout(Vector2T<int> position, float scale)
    {
        base.CalculateLayout(position, scale);
        
        _layoutDirty = false;
        
        foreach (AnchorControl control in Controls)
        {
            Vector2T<int> offset = (control.Offset.As<float>() * scale).As<int>();
            Size<int> layoutSize = ScreenRegion.Size;
            Size<int> size = (control.Control.Size.As<float>() * scale).As<int>();
            
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
            
            control.Control.CalculateLayout(position, scale);
        }
    }

    public readonly struct AnchorControl
    {
        public readonly Anchor Anchor;
        public readonly Vector2T<int> Offset;
        public readonly Control Control;

        public AnchorControl(Anchor anchor, Vector2T<int> offset, Control control)
        {
            Anchor = anchor;
            Offset = offset;
            Control = control;
        }
    }
}