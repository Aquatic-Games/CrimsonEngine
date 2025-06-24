using Crimson.Math;

namespace Crimson.Graphics.RHI;

public abstract class Texture : IDisposable
{
    public readonly Size<uint> Size;

    protected Texture(Size<uint> size)
    {
        Size = size;
    }

    public abstract void Dispose();
}