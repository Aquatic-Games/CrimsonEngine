global using JoltPhysics = JoltPhysicsSharp.PhysicsSystem;
using System.Diagnostics;
using System.Numerics;
using Crimson.Core;
using Crimson.Physics.Internal;
using JoltPhysicsSharp;

namespace Crimson.Physics;

public class PhysicsSystem : IDisposable
{
    private readonly JobSystem _jobSystem;
    
    internal readonly JoltPhysics Physics;

    public Vector3 Gravity
    {
        get => Physics.Gravity;
        set => Physics.Gravity = value;
    }
    
    public PhysicsSystem()
    {
        if (!Foundation.Init())
            throw new Exception("Failed to initialize Jolt Physics");
        
        Foundation.SetTraceHandler(TraceHandler);
        Foundation.SetAssertFailureHandler(AssertFailedHandler);

        PhysicsSystemSettings settings = new()
        {
            MaxBodies = ushort.MaxValue,
            MaxBodyPairs = ushort.MaxValue,
            MaxContactConstraints = ushort.MaxValue,
            NumBodyMutexes = 0,
        };

        ObjectLayerPairFilterTable objectFilter = new ObjectLayerPairFilterTable(2);
        objectFilter.EnableCollision(Layers.NonMoving, Layers.Moving);
        objectFilter.EnableCollision(Layers.Moving, Layers.Moving);

        BroadPhaseLayerInterfaceTable broadPhaseInterface = new BroadPhaseLayerInterfaceTable(2, 2);
        broadPhaseInterface.MapObjectToBroadPhaseLayer(Layers.NonMoving, Layers.NonMoving);
        broadPhaseInterface.MapObjectToBroadPhaseLayer(Layers.Moving, Layers.Moving);

        ObjectVsBroadPhaseLayerFilterTable objectBroadPhaseTable =
            new ObjectVsBroadPhaseLayerFilterTable(broadPhaseInterface, 2, objectFilter, 2);

        settings.ObjectLayerPairFilter = objectFilter;
        settings.BroadPhaseLayerInterface = broadPhaseInterface;
        settings.ObjectVsBroadPhaseLayerFilter = objectBroadPhaseTable;

        _jobSystem = new JobSystemThreadPool();

        Physics = new JoltPhysics(settings);
    }

    public void Step(float deltaTime)
    {
        PhysicsUpdateError error = Physics.Update(deltaTime, 1, _jobSystem);
        Debug.Assert(error == PhysicsUpdateError.None);
    }

    public Body CreateDynamicBody(Shape shape, Vector3 position, Quaternion rotation, float mass)
    {
        BodyCreationSettings bodySettings = new BodyCreationSettings(shape, position, rotation, MotionType.Dynamic, Layers.Moving)
        {
            MassPropertiesOverride = new MassProperties() { Mass = mass },
            OverrideMassProperties = OverrideMassProperties.CalculateInertia,
            Restitution = 0.2f
        };
        
        Body body = Physics.BodyInterface.CreateBody(bodySettings);
        
        Physics.BodyInterface.AddBody(body, Activation.Activate);

        return body;
    }

    public Body CreateStaticBody(Shape shape, Vector3 position, Quaternion rotation)
    {
        BodyCreationSettings bodySettings =
            new BodyCreationSettings(shape, position, rotation, MotionType.Static, Layers.NonMoving);

        Body body = Physics.BodyInterface.CreateBody(bodySettings);
        Physics.BodyInterface.AddBody(body, Activation.DontActivate);

        return body;
    }

    public void Dispose()
    {
        Physics.Dispose();
        Foundation.Shutdown();
    }
    
    private void TraceHandler(string message)
    {
        Logger.Trace(message);
    }
    
    private bool AssertFailedHandler(string expression, string message, string file, uint line)
    {
        Logger.Fatal(message, (int) line, file);
        throw new Exception(expression);
    }
}