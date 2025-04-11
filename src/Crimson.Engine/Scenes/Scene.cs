namespace Crimson.Engine.Scenes;

public abstract class Scene : IDisposable
{
    private readonly List<Entity> _entities;
    private readonly Dictionary<string, Entity> _entitiesMap;

    protected Scene()
    {
        _entities = [];
        _entitiesMap = [];
    }
    
    /// <summary>
    /// Called once, when the scene loads.
    /// </summary>
    public virtual void Initialize() { }

    /// <summary>
    /// Called once per frame. Put application logic in here.
    /// </summary>
    /// <param name="dt">The delta time since the last frame.</param>
    public virtual void Update(float dt) { }

    /// <summary>
    /// Called once per frame. Put rendering logic in here.
    /// </summary>
    public virtual void Draw() { }

    /// <summary>
    /// Dispose of this <see cref="Scene"/>.
    /// </summary>
    public virtual void Dispose() { }
}