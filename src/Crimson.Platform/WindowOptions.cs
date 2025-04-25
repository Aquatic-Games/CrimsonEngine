using Crimson.Math;

namespace Crimson.Platform;

/// <summary>
/// Describes how a <see cref="Surface"/> should be created for desktop platforms.
/// </summary>
/// <param name="Title">The title to use for the window.</param>
/// <param name="Size">The size, in pixels, of the window.</param>
/// <param name="Resizable">If the window should be resizable by the user.</param>
/// <param name="FullScreen">If the window should be borderless fullscreen.</param>
public record struct WindowOptions(string Title, Size<int> Size, bool Resizable, bool FullScreen)
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
    /// If the window should be resizable by the user.
    /// </summary>
    public bool Resizable = Resizable;

    /// <summary>
    /// If the window should be borderless fullscreen.
    /// </summary>
    public bool FullScreen = FullScreen;

    /// <summary>
    /// Create a new <see cref="WindowOptions"/> with the default values.
    /// </summary>
    public WindowOptions() : this("Crimson Engine", new Size<int>(1280, 720), false, false) { }
}