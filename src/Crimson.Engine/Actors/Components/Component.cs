namespace Crimson.Engine.Actors.Components;

/// <summary>
/// A component can be added to an <see cref="Actor"/> and can execute game logic.
/// </summary>
public abstract class Component : IDisposable
{
    /// <summary>
    /// Gets the <see cref="Actors.Actor"/> associated with the component.
    /// </summary>
    /// <remarks>
    /// This value is <see langword="null"/> until the component has been added to an actor.
    /// It has not been marked as nullable, as when used correctly, it is not possible for this value to be null.
    /// </remarks>
    public Actor Actor = null!;

    /// <summary>
    /// Gets the current <see cref="Actors.Transform"/>.
    /// </summary>
    protected ref Transform Transform => ref Actor.Transform;
    
    /// <summary>
    /// Runs once, when the component is initialized.
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
    /// Called when the component is disposed. Free resources in here.
    /// </summary>
    public virtual void Dispose() { }
}