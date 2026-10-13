using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Crimson.Engine.Actors.Components;

namespace Crimson.Engine.Actors;

/// <summary>
/// An actor is a scene object that contains logic and can have <see cref="Component"/>s added to it.
/// </summary>
public class Actor : IDisposable
{
    private bool _isInitialized;
    private readonly List<Component> _components;
    private readonly Dictionary<Type, Component> _componentTypes;

    private readonly List<Component> _componentsToAdd;
    
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

        _components = [];
        _componentTypes = [];

        _componentsToAdd = [];
    }

    /// <summary>
    /// Create an actor with the default transform.
    /// </summary>
    /// <param name="name">The name of the Actor, identifiable in the scene.</param>
    public Actor(string name) : this(name, new Transform()) { }

    /// <summary>
    /// Runs once, when the actor is initialized.
    /// </summary>
    public virtual void Init()
    {
        ReadOnlySpan<Component> components = CollectionsMarshal.AsSpan(_components);
        foreach (Component component in components)
            component.Init();

        _isInitialized = true;
    }

    /// <summary>
    /// Runs every tick. The tick rate is fixed and is denoted by <see cref="App.TicksPerSecond"/>.
    /// Put physics and logic requiring a fixed tick speed in here.
    /// </summary>
    /// <param name="dt">The time in seconds since the last tick. This value is fixed.</param>
    public virtual void Tick(float dt)
    {
        ReadOnlySpan<Component> components = CollectionsMarshal.AsSpan(_components);
        foreach (Component component in components)
            component.Tick(dt);
    }

    /// <summary>
    /// Runs every frame.
    /// Put input, per-frame logic, and rendering code in here.
    /// </summary>
    /// <param name="dt">The time in seconds since the last frame.</param>
    public virtual void Loop(float dt)
    {
        ReadOnlySpan<Component> components = CollectionsMarshal.AsSpan(_components);
        foreach (Component component in components)
            component.Loop(dt);

        if (_componentsToAdd.Count == 0)
            return;

        ReadOnlySpan<Component> componentsToAdd = CollectionsMarshal.AsSpan(_componentsToAdd);
        
        // resize the list and dict to make sure it holds everything,
        // without needing to potentially resize multiple times.
        Debug.Assert(_components.Count == _componentTypes.Count);
        _components.EnsureCapacity(_components.Count + componentsToAdd.Length);
        _componentTypes.EnsureCapacity(_componentTypes.Count + componentsToAdd.Length);
        
        foreach (Component component in componentsToAdd)
        {
            _components.Add(component);
            _componentTypes.Add(component.GetType(), component);
            component.Actor = this;
            component.Init();
        }
        
        _componentsToAdd.Clear();
    }

    /// <summary>
    /// Called when the actor is disposed. Free resources in here.
    /// </summary>
    public virtual void Dispose()
    {
        ReadOnlySpan<Component> components = CollectionsMarshal.AsSpan(_components);
        foreach (Component component in components)
            component.Dispose();
    }

    /// <summary>
    /// Tries to add a <see cref="Component"/> to the actor.
    /// </summary>
    /// <param name="component">The component to add.</param>
    /// <returns><see langword="false"/> if the component has already been added to the actor, otherwise <see langword="true"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryAddComponent(Component component)
    {
        if (_isInitialized)
        {
            if (_componentTypes.ContainsKey(component.GetType()))
                return false;
            
            _componentsToAdd.Add(component);
            return true;
        }
        
        Type type = component.GetType();
        bool result = _componentTypes.TryAdd(type, component);
        
        if (!result)
            return false;

        _components.Add(component);
        component.Actor = this;
        component.Init();
        return true;
    }

    /// <summary>
    /// Adds a <see cref="Component"/> to the actor.
    /// </summary>
    /// <param name="component">The component to add.</param>
    /// <remarks>
    /// If the component has already been added to the actor, an assertion error is thrown.
    /// In release, the result is ignored and the component will not be added.<br />
    /// Use <see cref="TryAddComponent"/> if you want to check the result at runtime.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Actor AddComponent(Component component)
    {
        bool result = TryAddComponent(component);
        Debug.Assert(result, "The component has already been added to the actor!");
        return this;
    }

    /// <summary>
    /// Tries to get a <see cref="Component"/> from the actor.
    /// </summary>
    /// <param name="component">The component, if any.</param>
    /// <typeparam name="T">The type of component to get.</typeparam>
    /// <returns><see langword="false"/> if the component has not been added to the actor, otherwise <see langword="true"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetComponent<T>([NotNullWhen(true)] out T? component) where T : Component
    {
        if (!_componentTypes.TryGetValue(typeof(T), out Component? comp))
        {
            component = null;
            return false;
        }

        component = (T) comp;
        return true;
    }

    /// <summary>
    /// Gets a <see cref="Component"/> from the actor.
    /// </summary>
    /// <typeparam name="T">The type of component to get.</typeparam>
    /// <returns>The component.</returns>
    /// <remarks>
    /// If the component has not been added to the actor, an assertion error is thrown.
    /// In release, this returns <see langword="null"/>.<br />
    /// Use <see cref="TryGetComponent"/> if you want to check the result at runtime.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetComponent<T>() where T : Component
    {
        bool result = TryGetComponent(out T? component);
        Debug.Assert(result, "The component has not been added to the actor!");
        return component!;
    }
}