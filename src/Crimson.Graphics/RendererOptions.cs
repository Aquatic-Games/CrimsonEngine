namespace Crimson.Graphics;

public record struct RendererOptions
{
    public RendererType Type;

    public bool Debug;

    public bool CreateImGuiRenderer;

    public RendererOptions(RendererType type, bool debug, bool createImGuiRenderer)
    {
        Type = type;
        Debug = debug;
        CreateImGuiRenderer = createImGuiRenderer;
    }
}