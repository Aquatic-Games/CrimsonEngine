using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Crimson.Engine.Entities;

public class Entity : IDisposable
{
    private readonly Dictionary<Type, Component> _components;
    private bool _isInitialized;

    private Dictionary<string, Entity> _children;
    
    public readonly string Name;

    public Transform Transform;

    public Entity? Parent { get; internal set; }

    public Matrix4x4 WorldMatrix => Transform.WorldMatrix * (Parent?.WorldMatrix ?? Matrix4x4.Identity);

    public Entity(string name, Transform transform)
    {
        Name = name;
        Transform = transform;

        _components = [];
        _isInitialized = false;
        _children = [];
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

    public bool TryAddChild(Entity child)
    {
        if (!_children.TryAdd(child.Name, child))
            return false;

        child.Parent = this;
        
        return true;
    }

    public void AddChild(Entity child)
    {
        if (!TryAddChild(child))
            throw new Exception($"Child with name '{child.Name}' has already been added to the entity.");
    }

    public bool TryGetChild(string name, out Entity? child)
    {
        if (name.Contains('/'))
            return TryResolveEntityPath(name, _children, out child);
        
        return _children.TryGetValue(name, out child);
    }

    public Entity GetChild(string name)
    {
        if (!TryGetChild(name, out Entity? child))
            throw new Exception($"Could not get child with name '{name}'.");

        return child!;
    }
    
    public virtual void Initialize()
    {
        Debug.Assert(_isInitialized == false);
        
        foreach ((_, Component component) in _components)
            component.Initialize();
        
        foreach ((_, Entity child) in _children)
            child.Initialize();

        _isInitialized = true;
    }

    public virtual void Update(float dt)
    {
        foreach ((_, Component component) in _components)
            component.Update(dt);
        
        foreach ((_, Entity child) in _children)
            child.Update(dt);
    }

    public virtual void Draw()
    {
        foreach ((_, Component component) in _components)
            component.Draw();
        
        foreach ((_, Entity child) in _children)
            child.Draw();
    }

    public virtual void Dispose()
    {
        foreach ((_, Component component) in _components)
            component.Dispose();
        
        foreach ((_, Entity child) in _children)
            child.Dispose();
    }

    internal static bool TryResolveEntityPath(string name, Dictionary<string, Entity> entityDict, out Entity? entity)
    {
        entity = null;
        
        int startIndex = 0;
        int endIndex;

        do
        {
            endIndex = name.IndexOf('/', startIndex);
            int index = endIndex == -1 ? name.Length : endIndex;

            string singleName = name[startIndex..index];
            if (!entityDict.TryGetValue(singleName, out entity))
                return false;

            entityDict = entity._children;
            
            startIndex = endIndex + 1;
        } while (endIndex != -1);
        
        return true;
    }
}