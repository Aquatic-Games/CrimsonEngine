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
    
    public readonly JoltPhysics Jolt;

    public Vector3 Gravity
    {
        get => Jolt.Gravity;
        set => Jolt.Gravity = value;
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

        Jolt = new JoltPhysics(settings);
    }

    public void Step(float deltaTime)
    {
        PhysicsUpdateError error = Jolt.Update(deltaTime, 1, _jobSystem);
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
        
        Body body = Jolt.BodyInterface.CreateBody(bodySettings);
        
        Jolt.BodyInterface.AddBody(body, Activation.Activate);

        return body;
    }

    public Body CreateStaticBody(Shape shape, Vector3 position, Quaternion rotation)
    {
        BodyCreationSettings bodySettings =
            new BodyCreationSettings(shape, position, rotation, MotionType.Static, Layers.NonMoving);

        Body body = Jolt.BodyInterface.CreateBody(bodySettings);
        Jolt.BodyInterface.AddBody(body, Activation.DontActivate);

        return body;
    }

    public bool Raycast(Vector3 position, Vector3 direction, float maxDistance, out RaycastHit hit)
    {
        Ray ray = new Ray(in position, direction * maxDistance);

        if (!Jolt.NarrowPhaseQuery.CastRay(in ray, out RayCastResult result))
        {
            hit = default;
            return false;
        }

        Jolt.BodyLockInterfaceNoLock.LockRead(in result.BodyID, out BodyLockRead read);

        // Is there a situation Body would be null?
        Debug.Assert(read.Body != null);
        
        Body body = read.Body;
        Vector3 hitPosition = ray.GetPointOnRay(result.Fraction);
        Vector3 normal = body.GetWorldSpaceSurfaceNormal(result.subShapeID2, hitPosition);

        hit = new RaycastHit()
        {
            BodyID = result.BodyID,
            HitPosition = hitPosition,
            SurfaceNormal = normal,
            ObjectPosition = body.Position
        };
        
        return true;
    }

    public void Dispose()
    {
        Jolt.Dispose();
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