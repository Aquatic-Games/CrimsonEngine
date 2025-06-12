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
    private readonly ThreadDispatcher _threadDispatcher;
    
    internal readonly Simulation Simulation;
    
    public Vector3 Gravity { get; set; }
    
    public PhysicsSystem()
    {
        _narrowPhaseCallbacks = new NarrowPhaseCallbacks();
        _poseIntegratorCallbacks = new PoseIntegratorCallbacks(new Vector3(0, -9.81f, 0));

        _bufferPool = new BufferPool();
        _threadDispatcher = new ThreadDispatcher(Environment.ProcessorCount);

        Logger.Trace("Creating simulation.");
        Simulation = Simulation.Create(_bufferPool, _narrowPhaseCallbacks, _poseIntegratorCallbacks,
            new SolveDescription(8, 1));
    }

    public void Step(float deltaTime)
    {
        Simulation.Timestep(deltaTime, _threadDispatcher);
    }

    public Body CreateBody(in BodyDescription description)
    {
        switch (description.Mobility)
        {
            case Mobility.Dynamic:
            {
                BepuPhysics.BodyDescription bepuDesc = BepuPhysics.BodyDescription.CreateDynamic(
                    new RigidPose(description.Position, description.Rotation),
                    description.Shape.CalculateInertia(description.Mass),
                    new CollidableDescription(description.Shape.Index), new BodyActivityDescription(0.01f));

                BodyHandle handle = Simulation.Bodies.Add(in bepuDesc);
                return new DynamicBody(Simulation, in handle);
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Dispose()
    {
        Simulation.Dispose();
    }
}