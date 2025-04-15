using System.Diagnostics;
using JoltPhysicsSharp;

namespace Crimson.Engine.Entities.Components;

public class Rigidbody : Component
{
    private readonly Shape _shape;
    private Body? _body;
    
    public Rigidbody(Shape shape)
    {
        _shape = shape;
    }

    public override void Initialize()
    {
        _body = App.Physics.CreateDynamicBody(_shape, Transform.Position, Transform.Rotation);
    }

    public override void Update(float dt)
    {
        Debug.Assert(_body != null);
        
        Transform.Position = _body.Position;
        Transform.Rotation = _body.Rotation;
    }
}