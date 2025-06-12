using System.Numerics;
using Crimson.Physics;
using Crimson.Physics.Shapes;

namespace Crimson.Engine.Entities.Components;

public class Rigidbody : Component
{
    private Body _body;
    
    public Rigidbody(Shape shape, float mass)
    {
        _body = App.Physics.CreateBody(new BodyDescription()
        {
            Shape = shape,
            Mobility = Mobility.Dynamic,
            Position = Vector3.Zero,
            Rotation = Quaternion.Identity,
            Mass = mass
        });
    }

    public override void Initialize()
    {
        _body.Position = Transform.Position;
        _body.Rotation = Transform.Rotation;
    }

    public override void Update(float dt)
    {
        Transform.Position = _body.Position;
        Transform.Rotation = _body.Rotation;
    }
}