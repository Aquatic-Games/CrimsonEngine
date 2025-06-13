using System.Numerics;
using System.Runtime.InteropServices;
using BepuPhysics;
using BepuPhysics.Collidables;

namespace Crimson.Physics.Shapes;

public class CompoundShape : Shape
{
    private List<float> _masses;

    internal CompoundShape(Simulation simulation, TypedIndex index, List<float> masses)
        : base(simulation, index)
    {
        _masses = masses;
    }
    
    protected internal override BodyInertia CalculateInertia(float mass)
    {
        ref BigCompound compound = ref Simulation.Shapes.GetShape<BigCompound>(Index.Index);
        
        return CompoundBuilder.ComputeInertia(compound.Children, CollectionsMarshal.AsSpan(_masses),
            Simulation.Shapes);
    }

    public void AddChild(in Child child)
    {
        ref BigCompound compound = ref Simulation.Shapes.GetShape<BigCompound>(Index.Index);
        
        compound.Add(new CompoundChild(new RigidPose(child.Position, child.Rotation), child.Shape.Index),
            Simulation.BufferPool, Simulation.Shapes);
        
        _masses.Add(child.Mass);
    }

    public Child GetChild(int index)
    {
        ref BigCompound compound = ref Simulation.Shapes.GetShape<BigCompound>(Index.Index);

        CompoundChild child = compound.GetChild(index);

        return new Child()
        {
            Position = child.LocalPosition,
            Rotation = child.LocalOrientation
        };
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