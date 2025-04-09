using Crimson.Platform;
using Version = Crimson.Core.Version;

namespace Crimson.Engine;

/// <summary>
/// Describes how an <see cref="App"/> should be created.
/// </summary>
/// <param name="Name">The app's name.</param>
/// <param name="Version">The app's version.</param>
public record struct AppOptions(string Name, Version Version)
{
    /// <summary>
    /// The app's name.
    /// </summary>
    public readonly string Name = Name;

    /// <summary>
    /// The app's version.
    /// </summary>
    public readonly Version Version = Version;

    /// <summary>
    /// The <see cref="WindowOptions"/> to use when the <see cref="Surface"/> is created.
    /// </summary>
    public WindowOptions Window = new WindowOptions() with { Title = Name };
}