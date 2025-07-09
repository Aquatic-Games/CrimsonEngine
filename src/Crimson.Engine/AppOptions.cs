using Crimson.Graphics;
using Crimson.Platform;
using Crimson.UI;

namespace Crimson.Engine;

/// <summary>
/// Describes how an <see cref="App"/> should be created.
/// </summary>
public record struct AppOptions
{
    /// <summary>
    /// The app's name.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// The app's version.
    /// </summary>
    public readonly string Version;

    /// <summary>
    /// The app type. This influences various things such as the default <see cref="Camera"/> type, and the default <see cref="RendererOptions"/>.
    /// </summary>
    public readonly AppType Type;

    /// <summary>
    /// The <see cref="WindowOptions"/> to use when the <see cref="Surface"/> is created.
    /// </summary>
    public WindowOptions Window;

    /// <summary>
    /// The <see cref="RendererOptions"/> to use when the <see cref="Crimson.Graphics.Renderer"/> is created.
    /// </summary>
    public RendererOptions Renderer;

    public UIOptions UI;

    /// Create the default <see cref="AppOptions"/>.
    /// <param name="name">The app's name.</param>
    /// <param name="version">The app's version.</param>
    /// <param name="type">The app type. This influences various things such as the default <see cref="Camera"/> type,
    /// and the default <see cref="RendererOptions"/>.</param>
    public AppOptions(string name, string version, AppType type = AppType.Type3D)
    {
        Name = name;
        Version = version;
        Type = type;

        Window = new WindowOptions() with { Title = Name };

        RendererType rendererType = type switch
        {
            AppType.TypeUI => RendererType.UiOnly,
            AppType.Type2D => RendererType.Create2D,
            AppType.Type3D => RendererType.Create3D,
            AppType.TypeBoth => RendererType.CreateBoth,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        
        Renderer = new RendererOptions
        {
            Type = rendererType,
#if DEBUG
            Debug = true,
#endif
            CreateImGuiRenderer = true
        };

        UI = new UIOptions();
    }
}