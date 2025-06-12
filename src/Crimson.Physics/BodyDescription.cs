using System.Numerics;
using Crimson.Physics.Shapes;

namespace Crimson.Physics;

public struct BodyDescription
{
    public Shape Shape;
    
    public Mobility Mobility;
    
    public Vector3 Position;

    public Quaternion Rotation;

    public float Mass;
}