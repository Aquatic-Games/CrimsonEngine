using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities;
using BepuUtilities.Memory;
using Crimson.Core;
using Crimson.Physics.Internal;
using Crimson.Physics.Shapes;
using Crimson.Physics.Shapes.Descriptions;

namespace Crimson.Physics;

public class PhysicsSystem : IDisposable
{
    private readonly NarrowPhaseCallbacks _narrowPhaseCallbacks;
    private readonly PoseIntegratorCallbacks _poseIntegratorCallbacks;

    private readonly BufferPool _bufferPool;
    
    internal readonly ThreadDispatcher ThreadDispatcher;
    internal readonly Simulation Simulation;
    
    public Vector3 Gravity { get; set; }
    
    public PhysicsSystem()
    {
        _narrowPhaseCallbacks = new NarrowPhaseCallbacks();
        _poseIntegratorCallbacks = new PoseIntegratorCallbacks(new Vector3(0, -9.81f, 0));

        _bufferPool = new BufferPool();
        ThreadDispatcher = new ThreadDispatcher(Environment.ProcessorCount);

        Logger.Trace("Creating simulation.");
        Simulation = Simulation.Create(_bufferPool, _narrowPhaseCallbacks, _poseIntegratorCallbacks,
            new SolveDescription(8, 1));
    }

    public void Step(float deltaTime)
    {
        Simulation.Timestep(deltaTime, ThreadDispatcher);
    }

    public Body CreateBody(in BodyDescription description)
    {
        RigidPose pose = new RigidPose(description.Position, description.Rotation);
        TypedIndex index = description.Shape.Index;
        
        switch (description.Mobility)
        {
            case Mobility.Dynamic:
            {
                BepuPhysics.BodyDescription bepuDesc = BepuPhysics.BodyDescription.CreateDynamic(pose,
                    description.Shape.CalculateInertia(description.Mass), new CollidableDescription(index),
                    new BodyActivityDescription(0.01f));

                BodyHandle handle = Simulation.Bodies.Add(in bepuDesc);
                return new DynamicBody(Simulation, in handle);
            }
            case Mobility.Kinematic:
                throw new NotImplementedException();
            case Mobility.Static:
            {
                StaticDescription bepuDesc = new StaticDescription(pose, index);
                StaticHandle handle = Simulation.Statics.Add(in bepuDesc);
                return new StaticBody(Simulation, handle);
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public bool Raycast(Vector3 position, Vector3 direction, float maxDistance, out RaycastHit hit)
    {
        RayHandler handler = new RayHandler();
        Simulation.RayCast(position, direction, maxDistance, ref handler);

        if (!handler.HasHit)
        {
            hit = default;
            return false;
        }

        Vector3 bodyPos;
        Quaternion bodyRot;

        switch (handler.Collidable.Mobility)
        {
            case CollidableMobility.Dynamic:
            case CollidableMobility.Kinematic:
            {
                BodyReference body = Simulation.Bodies.GetBodyReference(handler.Collidable.BodyHandle);
                bodyPos = body.Pose.Position;
                bodyRot = body.Pose.Orientation;
                
                break;
            }

            case CollidableMobility.Static:
            {
                StaticReference staticBody = Simulation.Statics.GetStaticReference(handler.Collidable.StaticHandle);
                bodyPos = staticBody.Pose.Position;
                bodyRot = staticBody.Pose.Orientation;
                
                break;
            }

            default:
                throw new ArgumentOutOfRangeException();
        }
        
        hit = new RaycastHit()
        {
            BodyID = handler.Collidable.Packed,
            WorldPosition = position + direction * handler.RayDistance,
            SurfaceNormal = handler.Normal,
            BodyPosition = bodyPos,
            BodyRotation = bodyRot,
            ChildIndex = handler.ChildIndex
        };
        
        return true;
    }

    public void Dispose()
    {
        Simulation.Dispose();
    }
}