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
    /// Called once per frame, before scene update.
    /// </summary>
    /// <param name="dt">The delta time since the last frame.</param>
    public virtual void PreUpdate(float dt) { }

    /// <summary>
    /// Called once per frame, after scene update.
    /// </summary>
    /// <param name="dt"></param>
    public virtual void PostUpdate(float dt) { }

    /// <summary>
    /// Called once per frame, before scene draw.
    /// </summary>
    public virtual void PreDraw() { }

    /// <summary>
    /// Called once per frame, after scene draw.
    /// </summary>
    public virtual void PostDraw() { }

    /// <summary>
    /// Dispose of this <see cref="GlobalApp"/>.
    /// </summary>
    public virtual void Dispose() { }
}