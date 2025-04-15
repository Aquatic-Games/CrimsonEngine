using System.Diagnostics;
using JoltPhysicsSharp;

namespace Crimson.Engine.Entities.Components;

public class Rigidbody : Component
{
    private readonly Shape _shape;
    private readonly float _mass;
    
    private Body? _body;
    
    public Rigidbody(Shape shape, float mass)
    {
        _shape = shape;
        _mass = mass;
    }

    public override void Initialize()
    {
        if (_mass == 0)
            _body = App.Physics.CreateStaticBody(_shape, Transform.Position, Transform.Rotation);
        else
            _body = App.Physics.CreateDynamicBody(_shape, Transform.Position, Transform.Rotation, _mass);
    }

    public override void Update(float dt)
    {
        Debug.Assert(_body != null);
        
        Transform.Position = _body.Position;
        Transform.Rotation = _body.Rotation;
    }
}