namespace Crimson.Engine.Entities;

public abstract class Component : IDisposable
{
    public Entity Entity { get; internal set; } = null!;

    protected ref Transform Transform => ref Entity.Transform;
    
    public virtual void Initialize() { }

    public virtual void Update(float dt) { }

    public virtual void Draw() { }

    public virtual void Dispose() { }

    protected bool TryAddComponent(Component component)
        => Entity.TryAddComponent(component);

    protected void AddComponent(Component component)
        => Entity.AddComponent(component);

    protected bool TryGetComponent<T>(out T? component) where T : Component
        => Entity.TryGetComponent(out component);
    
    protected void GetComponent<T>() where T : Component 
        => Entity.GetComponent<T>();

    protected bool TryAddEntity(Entity entity)
        => App.ActiveScene.TryAddEntity(entity);
    
    protected void AddEntity(Entity entity)
        => App.ActiveScene.AddEntity(entity);

    protected bool TryGetEntity(string name, out Entity? entity)
        => App.ActiveScene.TryGetEntity(name, out entity);
    
    protected Entity GetEntity(string name)
        => App.ActiveScene.GetEntity(name);
}