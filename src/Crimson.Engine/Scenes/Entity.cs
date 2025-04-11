namespace Crimson.Engine.Scenes;

public class Entity
{
    public readonly string Name;

    public Transform Transform;

    public Entity(string name)
    {
        Name = name;
        Transform = new Transform();
    }

    public Entity(string name, Transform transform)
    {
        Name = name;
        Transform = transform;
    }
    
    
}