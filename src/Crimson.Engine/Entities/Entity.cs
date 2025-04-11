using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Crimson.Engine.Entities;

public class Entity : IDisposable
{
    private readonly Dictionary<Type, Component> _components;
    private bool _isInitialized;
    
    public readonly string Name;

    public Transform Transform;

    public Entity(string name, Transform transform)
    {
        Name = name;
        Transform = transform;

        _components = [];
        _isInitialized = false;
    }

    public Entity(string name) : this(name, new Transform()) { }

    public bool TryAddComponent(Component component)
    {
        Type type = component.GetType();
        component.Entity = this;
        
        if (_isInitialized)
            component.Initialize();
        
        return _components.TryAdd(type, component);
    }

    public void AddComponent(Component component)
    {
        if (!TryAddComponent(component))
            throw new Exception($"Component of type {component.GetType()} is already added to the entity.");
    }

    public bool TryGetComponent<T>([NotNullWhen(true)] out T? component) where T : Component
    {
        Type type = typeof(T);
        component = null;
        if (!_components.TryGetValue(type, out Component? comp))
            return false;

        component = (T) comp;
        
        return true;
    }

    public T GetComponent<T>() where T : Component
    {
        if (!TryGetComponent(out T? component))
            throw new Exception($"Component of type {component.GetType()} has not been added to the entity.");

        return component;
    }
    
    public virtual void Initialize()
    {
        Debug.Assert(_isInitialized == false);
        
        foreach ((_, Component component) in _components)
            component.Initialize();

        _isInitialized = true;
    }

    public virtual void Update(float dt)
    {
        foreach ((_, Component component) in _components)
            component.Update(dt);
    }

    public virtual void Draw()
    {
        foreach ((_, Component component) in _components)
            component.Draw();
    }

    public virtual void Dispose()
    {
        foreach ((_, Component component) in _components)
            component.Dispose();
    }
}