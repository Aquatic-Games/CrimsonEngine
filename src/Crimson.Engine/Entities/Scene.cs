using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Crimson.Engine.Entities.Components;

namespace Crimson.Engine.Entities;

public abstract class Scene : IDisposable
{
    private readonly List<Entity> _entities;
    private readonly Dictionary<string, Entity> _entitiesMap;

    private readonly List<Entity> _entitiesToAdd;
    
    private bool _isInitialized;

    public Camera Camera => (Camera) GetEntity("Camera");

    protected Scene()
    {
        _entities = [];
        _entitiesMap = [];
        _isInitialized = false;

        _entitiesToAdd = [];
        
        AddEntity(new Camera("Camera", 75, 0.1f, 100f));
    }
    
    /// <summary>
    /// Try and add an <see cref="Entity"/> to the scene.
    /// </summary>
    /// <param name="entity">The <see cref="Entity"/> to add.</param>
    /// <returns>False if an entity with the same name has already been added, otherwise true.</returns>
    public bool TryAddEntity(Entity entity)
    {
        if (!_entitiesMap.TryAdd(entity.Name, entity))
            return false;
        
        if (_isInitialized)
            _entitiesToAdd.Add(entity);
        else
            _entities.Add(entity);

        return true;
    }

    /// <summary>
    /// Add an <see cref="Entity"/> to the scene.
    /// </summary>
    /// <param name="entity">The <see cref="Entity"/> to add.</param>
    /// <exception cref="Exception">Thrown if an entity with the same name has already been added.</exception>
    public void AddEntity(Entity entity)
    {
        if (!TryAddEntity(entity))
            throw new Exception($"An entity with name '{entity.Name}' has already been added to the scene.");
    }

    public bool TryGetEntity(string name, [NotNullWhen(true)] out Entity? entity)
    {
        if (name.Contains('/'))
            return Entity.TryResolveEntityPath(name, _entitiesMap, out entity);
        
        return _entitiesMap.TryGetValue(name, out entity);
    }

    public Entity GetEntity(string name)
    {
        if (!TryGetEntity(name, out Entity? entity))
            throw new Exception($"Entity with name '{name}' has not been added to the scene.");

        return entity;
    }

    /// <summary>
    /// Called once, when the scene loads. Entity initialization is performed here, so you should call this
    /// <b>after</b> adding entities to the scene.
    /// </summary>
    public virtual void Initialize()
    {
        Debug.Assert(_isInitialized == false);
        
        foreach (Entity entity in _entities)
            entity.Initialize();

        _isInitialized = true;
    }

    /// <summary>
    /// Called once per frame. Put application logic in here.
    /// </summary>
    /// <param name="dt">The delta time since the last frame.</param>
    public virtual void Update(float dt)
    {
        foreach (Entity entity in _entities)
            entity.Update(dt);

        foreach (Entity entity in _entitiesToAdd)
        {
            entity.Initialize();
            _entities.Add(entity);
        }

        _entitiesToAdd.Clear();
    }

    /// <summary>
    /// Called once per frame. Put rendering logic in here.
    /// </summary>
    public virtual void Draw()
    {
        foreach (Entity entity in _entities)
            entity.Draw();
    }

    /// <summary>
    /// Dispose of this <see cref="Scene"/>.
    /// </summary>
    public virtual void Dispose()
    {
        foreach (Entity entity in _entities)
            entity.Dispose();
    }
}