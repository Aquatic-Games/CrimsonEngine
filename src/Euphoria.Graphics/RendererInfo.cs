namespace Euphoria.Graphics;

public struct RendererInfo
{
    public Backend Backend;

    public bool Debug;

    public RendererInfo(Backend backend, bool debug)
    {
        Backend = backend;
        Debug = debug;
    }

    public static RendererInfo Default => new RendererInfo(Backend.Vulkan, true);
}