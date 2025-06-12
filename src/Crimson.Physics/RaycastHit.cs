using System.Numerics;

namespace Crimson.Physics;

public struct RaycastHit
{
    public uint BodyID;

    /// <summary>
    /// The world-space position that the hit occurred.
    /// </summary>
    public Vector3 HitPosition;

    /// <summary>
    /// The object normal that the hit occurred on.
    /// </summary>
    public Vector3 SurfaceNormal;

    /// <summary>
    /// The world space position of the body the ray hit.
    /// </summary>
    public Vector3 BodyPosition;

    public Quaternion BodyRotation;
}