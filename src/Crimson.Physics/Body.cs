using System.Numerics;
using BepuPhysics;

namespace Crimson.Physics;

public abstract class Body
{
    internal readonly Simulation Simulation;
    
    public abstract Vector3 Position { get; set; }
    
    public abstract Quaternion Rotation { get; set; }

    protected Body(Simulation simulation)
    {
        Simulation = simulation;
    }
}