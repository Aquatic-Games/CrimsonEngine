using Euphoria.Windowing;
using Version = Euphoria.Core.Version;

namespace Euphoria.Engine;

public record struct AppOptions(string Name, Version Version)
{
    public readonly string Name = Name;

    public readonly Version Version = Version;

    public WindowOptions Window = new WindowOptions() with { Title = Name };
}