namespace Crimson.Engine;

/// <summary>
/// A global application, used to perform tasks globally and also allows you to control the update and render loops.
/// </summary>
public class GlobalApp : IDisposable
{
    /// <summary>
    /// Called once, when the application starts.
    /// </summary>
    public virtual void Initialize() { }

    /// <summary>
    /// Called once per frame. Put application logic in here.
    /// </summary>
    /// <param name="dt">The delta time since the last frame.</param>
    public virtual void Update(float dt) { }

    /// <summary>
    /// Called once per frame. Put rendering logic in here.
    /// </summary>
    public virtual void Draw() { }

    /// <summary>
    /// Dispose of this <see cref="GlobalApp"/>.
    /// </summary>
    public virtual void Dispose() { }
}