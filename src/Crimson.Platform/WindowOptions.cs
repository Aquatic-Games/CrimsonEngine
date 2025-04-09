using Crimson.Math;

namespace Crimson.Platform;

/// <summary>
/// Describes how a <see cref="Surface"/> should be created for desktop platforms.
/// </summary>
/// <param name="Title">The title to use for the window.</param>
/// <param name="Size">The size, in pixels, of the window.</param>
public record struct WindowOptions(string Title, Size<int> Size)
{
    /// <summary>
    /// The title to use for the window.
    /// </summary>
    public string Title = Title;

    /// <summary>
    /// The size, in pixels, of the window.
    /// </summary>
    public Size<int> Size = Size;

    /// <summary>
    /// Create a new <see cref="WindowOptions"/> with the default values.
    /// </summary>
    public WindowOptions() : this("Crimson Engine", new Size<int>(1280, 720)) { }
}