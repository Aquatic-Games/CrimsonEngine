using Euphoria.Math;

namespace Euphoria.Engine;

public struct WindowInfo
{
    public string Title;

    public Size<int> Size;

    public WindowInfo(string title, Size<int> size)
    {
        Title = title;
        Size = size;
    }

    public static WindowInfo Default => new WindowInfo("Euphoria Application", new Size<int>(1280, 720));
}