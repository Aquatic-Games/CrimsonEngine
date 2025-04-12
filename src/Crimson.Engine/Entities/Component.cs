namespace Crimson.Engine.Entities;

public abstract class Component : IDisposable
{
    public Entity Entity { get; internal set; }

    protected ref Transform Transform => ref Entity.Transform;
    
    public virtual void Initialize() { }

    public virtual void Update(float dt) { }

    public virtual void Draw() { }

    public virtual void Dispose() { }
}