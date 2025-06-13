using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;

namespace Crimson.Physics;

internal sealed class StaticBody : Body
{
    private readonly StaticHandle _handle;

    public override uint ID => new CollidableReference(_handle).Packed;

    public override Vector3 Position
    {
        get => Simulation.Statics.GetStaticReference(_handle).Pose.Position;
        set
        {
            Simulation.Statics.GetStaticReference(_handle).Pose.Position = value;
            Simulation.Statics.UpdateBounds(_handle);
        }
    }

    public override Quaternion Rotation
    {
        get => Simulation.Statics.GetStaticReference(_handle).Pose.Orientation;
        set
        {
            Simulation.Statics.GetStaticReference(_handle).Pose.Orientation = value;
            Simulation.Statics.UpdateBounds(_handle);
        }
    }

    public StaticBody(Simulation simulation, StaticHandle handle) : base(simulation)
    {
        _handle = handle;
    }
    
    public override void UpdateBounds()
    {
        Simulation.Statics[_handle].UpdateBounds();
    }

    public override void Dispose()
    {
        Simulation.Statics.Remove(_handle);
    }
}