using BepuPhysics;
using BepuPhysics.Collidables;
using Crimson.Physics.Shapes.Descriptions;

namespace Crimson.Physics.Shapes;

public sealed class BoxShape : Shape
{
    internal BoxShape(Simulation simulation, TypedIndex index) : base(simulation, index) { }
    
    protected internal override BodyInertia CalculateInertia(float mass)
    {
        return Simulation.Shapes.GetShape<Box>(Index.Index).ComputeInertia(mass);
    }
}