namespace Crimson.Engine;

public enum AppType
{
    /// <summary>
    /// The app will be a UI-only app.
    /// </summary>
    TypeUI,
    
    /// <summary>
    /// The app will be a 2D-only app.
    /// </summary>
    Type2D,
    
    /// <summary>
    /// The app will be a 3D-only app.
    /// </summary>
    Type3D,
    
    /// <summary>
    /// The app will be both a 2D and 3D app.
    /// </summary>
    TypeBoth
}