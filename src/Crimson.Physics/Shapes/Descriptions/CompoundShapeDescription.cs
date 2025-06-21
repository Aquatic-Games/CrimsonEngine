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
    
    public CompoundShape Create()
    {
        Debug.Assert(Children.Count > 0);
        
        CompoundBuilder builder =
            new CompoundBuilder(Physics.Simulation.BufferPool, Physics.Simulation.Shapes, Children.Count);

        List<float> masses = new List<float>(Children.Count);
        
        foreach (CompoundShape.Child child in Children)
        {
            builder.Add(child.Shape.Index, new RigidPose(child.Position, child.Rotation),
                child.Shape.CalculateInertia(child.Mass));
            
            masses.Add(child.Mass);
        }
        
        builder.BuildDynamicCompound(out Buffer<CompoundChild> children, out BodyInertia inertia);
        builder.Dispose();

        BigCompound compound = new BigCompound(children, Physics.Simulation.Shapes, Physics.Simulation.BufferPool, Physics.ThreadDispatcher);
        TypedIndex index = Physics.Simulation.Shapes.Add(compound);
        
        return new CompoundShape(Physics.Simulation, index, masses);
    }
}