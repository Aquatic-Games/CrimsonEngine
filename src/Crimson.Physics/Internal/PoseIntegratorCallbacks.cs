using System.Numerics;
using BepuPhysics;
using BepuUtilities;

namespace Crimson.Physics.Internal;

public struct PoseIntegratorCallbacks : IPoseIntegratorCallbacks
{
    private Vector3Wide _gravityWideDt;
    
    public Vector3 Gravity;
    
    public AngularIntegrationMode AngularIntegrationMode => AngularIntegrationMode.Nonconserving;

    public bool AllowSubstepsForUnconstrainedBodies => false;

    public bool IntegrateVelocityForKinematics => false;

    public PoseIntegratorCallbacks(Vector3 gravity)
    {
        Gravity = gravity;
    }

    public void Initialize(Simulation simulation) { }

    public void PrepareForIntegration(float dt)
    {
        _gravityWideDt = Vector3Wide.Broadcast(Gravity * dt);
    }

    public void IntegrateVelocity(Vector<int> bodyIndices, Vector3Wide position, QuaternionWide orientation,
        BodyInertiaWide localInertia, Vector<int> integrationMask, int workerIndex, Vector<float> dt, ref BodyVelocityWide velocity)
    {
        velocity.Linear += _gravityWideDt;
    }
}