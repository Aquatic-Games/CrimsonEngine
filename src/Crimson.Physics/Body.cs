using System.Numerics;
using BepuPhysics;

namespace Crimson.Physics;

public abstract class Body : IDisposable
{
    internal readonly Simulation Simulation;
    
    public abstract uint ID { get; }
    
    public abstract Vector3 Position { get; set; }
    
    public abstract Quaternion Rotation { get; set; }

    public abstract void UpdateBounds();

    protected Body(Simulation simulation)
    {
        Simulation = simulation;
    }

    public abstract void Dispose();
}