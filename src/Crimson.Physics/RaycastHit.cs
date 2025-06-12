using System.Numerics;

namespace Crimson.Physics;

public struct RaycastHit
{
    public uint BodyID;

    /// <summary>
    /// The world-space position that the hit occurred.
    /// </summary>
    public Vector3 WorldPosition;

    /// <summary>
    /// The surface normal that the hit occurred on.
    /// </summary>
    public Vector3 SurfaceNormal;

    /// <summary>
    /// The world space position of the body the ray hit.
    /// </summary>
    public Vector3 BodyPosition;

    /// <summary>
    /// The rotation of the body that the ray hit.
    /// </summary>
    public Quaternion BodyRotation;

    /// <summary>
    /// The child index of the hit, if any. Used for shapes with children, such as compound shapes.
    /// </summary>
    public int ChildIndex;
}