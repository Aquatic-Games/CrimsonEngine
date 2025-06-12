using System.Numerics;
using BepuPhysics;

namespace Crimson.Physics;

internal sealed class StaticBody : Body
{
    private readonly StaticHandle _handle;

    public override Vector3 Position
    {
        get => Simulation.Statics.GetStaticReference(_handle).Pose.Position;
        set
        {
            Simulation.Statics.GetStaticReference(_handle).Pose.Position = value;
            Simulation.Statics.UpdateBounds(_handle);
        }
    }
    
    public override Quaternion Rotation { get; set; }

    public StaticBody(Simulation simulation, StaticHandle handle) : base(simulation)
    {
        _handle = handle;
    }
}