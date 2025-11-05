using Crimson.Math;

namespace Crimson.Platform;

public struct VideoMode
{
    public Size<int> Size;

    public float RefreshRate;

    public VideoMode(Size<int> size, float refreshRate)
    {
        Size = size;
        RefreshRate = refreshRate;
    }
}