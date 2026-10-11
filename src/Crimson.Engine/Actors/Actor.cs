namespace Crimson.Engine.Actors;

/// <summary>
/// An actor is a scene object that contains logic and can have <see cref="Component"/>s added to it.
/// </summary>
public class Actor : IDisposable
{
    /// <summary>
    /// The name of the Actor, identifiable in the scene.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// The transform of this Actor.
    /// </summary>
    public Transform Transform;
    
    /// <summary>
    /// Create an Actor with a transform.
    /// </summary>
    /// <param name="name">The name of the Actor, identifiable in the scene.</param>
    /// <param name="transform">The transform of this Actor.</param>
    public Actor(string name, Transform transform)
    {
        Name = name;
        Transform = transform;
    }

    /// <summary>
    /// Create an actor with the default transform.
    /// </summary>
    /// <param name="name">The name of the Actor, identifiable in the scene.</param>
    public Actor(string name) : this(name, new Transform()) { }

    /// <summary>
    /// Runs once, when the actor is initialized.
    /// </summary>
    public virtual void Init() { }

    /// <summary>
    /// Runs every tick. The tick rate is fixed and is denoted by <see cref="App.TicksPerSecond"/>.
    /// Put physics and logic requiring a fixed tick speed in here.
    /// </summary>
    /// <param name="dt">The time in seconds since the last tick. This value is fixed.</param>
    public virtual void Tick(float dt) { }

    /// <summary>
    /// Runs every frame.
    /// Put input, per-frame logic, and rendering code in here.
    /// </summary>
    /// <param name="dt">The time in seconds since the last frame.</param>
    public virtual void Loop(float dt) { }

    /// <summary>
    /// Called when the actor is disposed. Free resources in here.
    /// </summary>
    public virtual void Dispose() { }
}