using Euphoria.Math;

namespace Euphoria.Windowing;

public record struct WindowOptions(string Title, Size<int> Size)
{
    public string Title = Title;

    public Size<int> Size = Size;

    public WindowOptions() : this("Euphoria", new Size<int>(1280, 720)) { }
}