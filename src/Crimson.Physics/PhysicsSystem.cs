using System.Numerics;
using BepuPhysics;
using BepuUtilities;
using BepuUtilities.Memory;
using Crimson.Core;
using Crimson.Physics.Internal;

namespace Crimson.Physics;

public class PhysicsSystem : IDisposable
{
    private readonly NarrowPhaseCallbacks _narrowPhaseCallbacks;
    private readonly PoseIntegratorCallbacks _poseIntegratorCallbacks;

    private readonly BufferPool _bufferPool;
    private readonly ThreadDispatcher _threadDispatcher;
    
    public readonly Simulation Simulation;
    
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

    /*public Body CreateDynamicBody(Shape shape, Vector3 position, Quaternion rotation, float mass)
    {
        throw new NotImplementedException();
    }

    public Body CreateStaticBody(Shape shape, Vector3 position, Quaternion rotation)
    {
        throw new NotImplementedException();
    }*/

    public void Dispose()
    {
        Simulation.Dispose();
    }
}