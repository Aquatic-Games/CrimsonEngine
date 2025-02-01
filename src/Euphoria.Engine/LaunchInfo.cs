using Euphoria.Graphics;

namespace Euphoria.Engine;

public struct LaunchInfo
{
    public readonly string AppName;

    public readonly Version Version;

    public WindowInfo Window;

    public RendererInfo Renderer;

    public LaunchInfo(string appName, Version version, WindowInfo window, RendererInfo renderer)
    {
        AppName = appName;
        Version = version;
        Window = window;
        Renderer = renderer;
    }

    public static LaunchInfo Default(string appName, Version version) =>
        new LaunchInfo(appName, version, WindowInfo.Default with { Title = appName }, RendererInfo.Default);
}