using BepuPhysics;
using BepuPhysics.Collidables;

namespace Crimson.Physics.Shapes;

public abstract class Shape
{
    internal readonly Simulation Simulation;
    internal readonly TypedIndex Index;
    
    protected Shape(Simulation simulation, TypedIndex index)
    {
        Simulation = simulation;
        Index = index;
    }

    protected internal abstract BodyInertia CalculateInertia(float mass);
}