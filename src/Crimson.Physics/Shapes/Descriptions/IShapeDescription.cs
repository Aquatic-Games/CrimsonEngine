using BepuPhysics;

namespace Crimson.Physics.Shapes.Descriptions;

public interface IShapeDescription<TShape> where TShape : Shape
{
    public TShape Create(PhysicsSystem physics);
}