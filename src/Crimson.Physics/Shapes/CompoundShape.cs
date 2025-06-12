using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;

namespace Crimson.Physics.Shapes;

public class CompoundShape : Shape
{
    private BigCompound _compound;

    internal CompoundShape(Simulation simulation, TypedIndex index, BigCompound compound) : base(simulation, index)
    {
        _compound = compound;
    }
    
    protected internal override BodyInertia CalculateInertia(float mass)
    {
        throw new NotImplementedException();
    }

    public void AddChild(in Child child)
    {
        _compound.Add(new CompoundChild(new RigidPose(child.Position, child.Rotation), child.Shape.Index),
            Simulation.BufferPool, Simulation.Shapes);
    }

    public struct Child
    {
        public Shape Shape;
        
        public Vector3 Position;

        public Quaternion Rotation;

        public float Mass;

        public Child(Shape shape, Vector3 position, Quaternion rotation, float mass)
        {
            Shape = shape;
            Position = position;
            Rotation = rotation;
            Mass = mass;
        }
    }
}