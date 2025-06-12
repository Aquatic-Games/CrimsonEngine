using System.Diagnostics;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities.Memory;

namespace Crimson.Physics.Shapes.Descriptions;

public struct CompoundShapeDescription : IShapeDescription<CompoundShape>
{
    public List<CompoundShape.Child> Children;

    public CompoundShapeDescription()
    {
        Children = [];
    }
    
    public CompoundShape Create(PhysicsSystem physics)
    {
        Debug.Assert(Children.Count > 0);
        
        CompoundBuilder builder =
            new CompoundBuilder(physics.Simulation.BufferPool, physics.Simulation.Shapes, Children.Count);

        foreach (CompoundShape.Child child in Children)
        {
            builder.Add(child.Shape.Index, new RigidPose(child.Position, child.Rotation),
                child.Shape.CalculateInertia(child.Mass));
        }
        
        builder.BuildDynamicCompound(out Buffer<CompoundChild> children, out BodyInertia inertia);
        builder.Dispose();

        BigCompound compound = new BigCompound(children, physics.Simulation.Shapes, physics.Simulation.BufferPool, physics.ThreadDispatcher);
        TypedIndex index = physics.Simulation.Shapes.Add(compound);
        return new CompoundShape(physics.Simulation, index, compound);
    }
}