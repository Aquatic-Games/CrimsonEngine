namespace Crimson.Graphics;

/// <summary>
/// Defines various ways the renderer can be created.
/// </summary>
[Flags]
public enum RendererType
{
    /// <summary>
    /// Don't create a 2D or 3D renderer.
    /// </summary>
    UiOnly = 0,
    
    /// <summary>
    /// Create the 3D renderer. This is the default behaviour.
    /// </summary>
    Create3D = 1 << 0,
    
    /// <summary>
    /// Create the 2D renderer.
    /// </summary>
    Create2D = 1 << 1,
    
    /// <summary>
    /// Create both the 3D and 2D renderers.
    /// </summary>
    CreateBoth = Create3D | Create2D
}