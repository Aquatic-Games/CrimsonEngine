global using JoltPhysics = JoltPhysicsSharp.PhysicsSystem;
using System.Diagnostics;
using Crimson.Core;
using JoltPhysicsSharp;

namespace Crimson.Physics;

public class PhysicsSystem : IDisposable
{
    private readonly JobSystem _jobSystem;
    
    internal readonly JoltPhysics Physics;
    
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
            NumBodyMutexes = 0
        };

        _jobSystem = new JobSystemThreadPool();

        Physics = new JoltPhysics(settings);
    }

    public void Step(float deltaTime)
    {
        PhysicsUpdateError error = Physics.Update(deltaTime, 1, _jobSystem);
        Debug.Assert(error == PhysicsUpdateError.None);
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