namespace Crimson.Graphics;

public record struct RendererOptions
{
    public RendererType Type;

    public Backend Backend;

    public bool Debug;

    public ImGuiInfo ImGui;

    public RendererOptions(RendererType type, Backend backend, bool debug, ImGuiInfo imgui)
    {
        Type = type;
        Backend = backend;
        Debug = debug;
        ImGui = imgui;
    }

    public struct ImGuiInfo
    {
        public bool CreateRenderer;

        public string? Font;

        public uint? FontSize;

        public ImGuiInfo()
        {
            CreateRenderer = true;
            Font = null;
            FontSize = null;
        }
        
        public ImGuiInfo(bool createRenderer, string? font = null, uint? fontSize = null)
        {
            CreateRenderer = createRenderer;
            Font = font;
            FontSize = fontSize;
        }
    }
}