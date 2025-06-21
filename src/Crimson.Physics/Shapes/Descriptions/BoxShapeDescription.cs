using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;

namespace Crimson.Physics.Shapes.Descriptions;

public struct BoxShapeDescription : IShapeDescription<BoxShape>
{
    public Vector3 HalfExtents;

    public BoxShapeDescription(Vector3 halfExtents)
    {
        HalfExtents = halfExtents;
    }

    public BoxShape Create()
    {
        Simulation simulation = Physics.Simulation;
        
        TypedIndex index = simulation.Shapes.Add(new Box()
            { HalfWidth = HalfExtents.X, HalfLength = HalfExtents.Y, HalfHeight = HalfExtents.Z });

        return new BoxShape(simulation, index);
    }
}