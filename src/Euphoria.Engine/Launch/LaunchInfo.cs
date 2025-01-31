namespace Euphoria.Engine.Launch;

public struct LaunchInfo
{
    public readonly string AppName;

    public readonly Version Version;

    public WindowInfo Window;

    public LaunchInfo(string appName, Version version, WindowInfo window)
    {
        AppName = appName;
        Version = version;
        Window = window;
    }

    public static LaunchInfo Default(string appName, Version version) =>
        new LaunchInfo(appName, version, WindowInfo.Default with { Title = appName });
}