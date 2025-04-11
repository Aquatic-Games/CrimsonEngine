using System.Diagnostics;

namespace Crimson.Engine.Entities;

public abstract class Scene : IDisposable
{
    private readonly List<Entity> _entities;
    private readonly Dictionary<string, Entity> _entitiesMap;
    private bool _isInitialized;

    protected Scene()
    {
        _entities = [];
        _entitiesMap = [];
        _isInitialized = false;
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