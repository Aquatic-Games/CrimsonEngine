using Crimson.Math;

namespace Crimson.UI;

public struct UIOptions
{
    public string? DefaultFont;

    public ScaleMethod ScaleMethod;

    public Size<int> ReferenceSize;

    public UIOptions()
    {
        DefaultFont = null;
        ScaleMethod = ScaleMethod.Manual;
        ReferenceSize = new Size<int>(1280, 720);
    }
    
    public UIOptions(string? defaultFont, ScaleMethod scaleMethod, Size<int> referenceSize)
    {
        DefaultFont = defaultFont;
        ScaleMethod = scaleMethod;
        ReferenceSize = referenceSize;
    }
}