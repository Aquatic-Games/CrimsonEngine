namespace Euphoria.Graphics;

public struct RendererInfo
{
    public bool Debug;

    public RendererInfo(bool debug)
    {
        Debug = debug;
    }

    public static RendererInfo Default => new RendererInfo(true);
}