namespace Crimson.Graphics.Materials;

/// <summary>
/// Defines which face(s) should be rendered.
/// </summary>
public enum RenderFace
{
    /// <summary>
    /// Render only the front faces.
    /// </summary>
    Front,
    
    /// <summary>
    /// Render only the back faces.
    /// </summary>
    Back,
    
    /// <summary>
    /// Render both faces.
    /// </summary>
    Both
}