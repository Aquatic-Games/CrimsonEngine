using Euphoria.Math;

namespace Euphoria.Engine.Launch;

public struct WindowInfo
{
    public string Title;

    public Size<int> Size;

    public WindowInfo(string title, Size<int> size)
    {
        Title = title;
        Size = size;
    }
}