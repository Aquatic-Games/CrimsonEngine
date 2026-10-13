using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
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

    /// <summary>
    /// Gets the main camera.
    /// </summary>
    public Camera Camera => GetActor<Camera>("Camera"); // todo should have a way to rename the camera.

    protected Scene()
    {
        _actors = [];
        _actorNames = [];
        _actorsToAdd = [];
        
        AddActor(new Camera("Camera", float.DegreesToRadians(75)));
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
        
        if (_actorsToAdd.Count == 0)
            return;
        
        ReadOnlySpan<Actor> actorsToAdd = CollectionsMarshal.AsSpan(_actorsToAdd);
        
        // resize the list and dict to make sure it holds everything,
        // without needing to potentially resize multiple times.
        Debug.Assert(_actors.Count == _actorNames.Count);
        _actors.EnsureCapacity(_actors.Count + actorsToAdd.Length);
        _actorNames.EnsureCapacity(_actorNames.Count + actorsToAdd.Length);
        
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

    /// <summary>
    /// Tries to get an <see cref="Actor"/> from the scene.
    /// </summary>
    /// <param name="name">The name of the actor.</param>
    /// <param name="actor">The actor, if any.</param>
    /// <returns><see langword="false"/> if the actor was not in the scene, otherwise <see langword="true"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetActor(string name, [NotNullWhen(true)] out Actor? actor) 
        => _actorNames.TryGetValue(name, out actor);

    /// <summary>
    /// Gets an <see cref="Actor"/> from the scene.
    /// </summary>
    /// <param name="name">The name of the actor.</param>
    /// <returns>The actor.</returns>
    /// <remarks>
    /// If the actor has not been added to the scene, an assertion error is thrown.
    /// In release, this returns <see langword="null"/>.<br />
    /// Use <see cref="TryGetActor"/> if you want to check the result at runtime.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Actor GetActor(string name)
    {
        bool result = TryGetActor(name, out Actor? actor);
        Debug.Assert(result, "The actor has not been added to the scene!");
        return actor!;
    }

    /// <summary>
    /// Tries to get an <see cref="Actor"/> with the specified type from the scene.
    /// </summary>
    /// <param name="name">The name of the actor.</param>
    /// <param name="actor">The actor, if any.</param>
    /// <typeparam name="T">The actor's type.</typeparam>
    /// <returns><see langword="false"/> if the actor was not in the scene, otherwise <see langword="true"/></returns>
    public bool TryGetActor<T>(string name, [NotNullWhen(true)] out T? actor) where T : Actor
    {
        if (!_actorNames.TryGetValue(name, out Actor? a))
        {
            actor = null;
            return false;
        }

        actor = (T) a;
        return true;
    }

    /// <summary>
    /// Gets an <see cref="Actor"/> with the specified type from the scene.
    /// </summary>
    /// <param name="name">The name of the actor.</param>
    /// <returns>The actor.</returns>
    /// <typeparam name="T">The actor's type.</typeparam>
    /// <remarks>
    /// If the actor has not been added to the scene, an assertion error is thrown.
    /// In release, this returns <see langword="null"/>.<br />
    /// Use <see cref="TryGetActor"/> if you want to check the result at runtime.
    /// </remarks>
    public T GetActor<T>(string name) where T : Actor
    {
        bool result = TryGetActor(name, out T? actor);
        Debug.Assert(result, "The actor has not been added to the scene!");
        return actor!;
    }
}