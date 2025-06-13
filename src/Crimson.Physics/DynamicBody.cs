using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;

namespace Crimson.Physics;

internal sealed class DynamicBody : Body
{
    private readonly BodyHandle _handle;

    public override uint ID => new CollidableReference(CollidableMobility.Dynamic, _handle).Packed;

    public override Vector3 Position
    {
        get => Simulation.Bodies.GetBodyReference(_handle).Pose.Position;
        set => Simulation.Bodies.GetBodyReference(_handle).Pose.Position = value;
    }

    public override Quaternion Rotation
    {
        get => Simulation.Bodies.GetBodyReference(_handle).Pose.Orientation;
        set => Simulation.Bodies.GetBodyReference(_handle).Pose.Orientation = value;
    }
    
    public DynamicBody(Simulation simulation, in BodyHandle handle) : base(simulation)
    {
        _handle = handle;
    }

    public override void UpdateBounds()
    {
        BodyReference reference = Simulation.Bodies[_handle];
        reference.Awake = true;
        reference.UpdateBounds();
    }
}