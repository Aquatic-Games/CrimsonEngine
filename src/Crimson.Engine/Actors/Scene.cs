using System.Runtime.InteropServices;

namespace Crimson.Engine.Actors;

/// <summary>
/// Scenes contain a list of <see cref="Actor"/>s.
/// </summary>
public abstract class Scene : IDisposable
{
    private readonly List<Actor> _actors;
    private readonly Dictionary<string, Actor> _actorNames;

    private bool _isInitialized;
    private readonly List<Actor> _actorsToAdd;

    protected Scene()
    {
        _actors = [];
        _actorNames = [];
        _actorsToAdd = [];
    }

    /// <summary>
    /// Runs once, when the scene is initialized.
    /// </summary>
    public virtual void Init()
    {
        ReadOnlySpan<Actor> actors = CollectionsMarshal.AsSpan(_actors);
        foreach (Actor actor in actors)
            actor.Init();

        _isInitialized = true;
    }

    /// <summary>
    /// Runs every tick. The tick rate is fixed and is denoted by <see cref="App.TicksPerSecond"/>.
    /// Put physics and logic requiring a fixed tick speed in here.
    /// </summary>
    /// <param name="dt">The time in seconds since the last tick. This value is fixed.</param>
    public virtual void Tick(float dt)
    {
        ReadOnlySpan<Actor> actors = CollectionsMarshal.AsSpan(_actors);
        foreach (Actor actor in actors)
            actor.Tick(dt);
    }

    /// <summary>
    /// Runs every frame.
    /// Put input, per-frame logic, and rendering code in here.
    /// </summary>
    /// <param name="dt">The time in seconds since the last frame.</param>
    public virtual void Loop(float dt)
    {
        ReadOnlySpan<Actor> actors = CollectionsMarshal.AsSpan(_actors);
        foreach (Actor actor in actors)
            actor.Loop(dt);

        ReadOnlySpan<Actor> actorsToAdd = CollectionsMarshal.AsSpan(_actorsToAdd);
        if (actorsToAdd.IsEmpty)
            return;
        
        foreach (Actor actor in actorsToAdd)
        {
            _actors.Add(actor);
            _actorNames.Add(actor.Name, actor);
            actor.Init();
        }
        _actors.Clear();
    }

    /// <summary>
    /// Called when the scene is disposed. Free resources in here.
    /// </summary>
    public virtual void Dispose()
    {
        ReadOnlySpan<Actor> actors = CollectionsMarshal.AsSpan(_actors);
        foreach (Actor actor in actors)
            actor.Dispose();
    }

    /// <summary>
    /// Adds an <see cref="Actor"/> to the scene.
    /// </summary>
    /// <param name="actor">The <see cref="Actor"/> to add to the scene.</param>
    public void AddActor(Actor actor)
    {
        if (_isInitialized)
            _actorsToAdd.Add(actor);
        else
        {
            _actors.Add(actor);
            _actorNames.Add(actor.Name, actor);
        }
    }
}