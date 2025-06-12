using System.Numerics;
using JoltPhysicsSharp;

namespace Crimson.Physics;

/// <summary>
/// Contains data about a raycast hit.
/// </summary>
public struct RaycastHit
{
    public BodyID BodyID;
    
    /// <summary>
    /// The world-space position that the hit occurred.
    /// </summary>
    public Vector3 HitPosition;

    /// <summary>
    /// The object normal that the hit occurred on.
    /// </summary>
    public Vector3 SurfaceNormal;

    /// <summary>
    /// The world space position of the object the ray hit.
    /// </summary>
    public Vector3 ObjectPosition;
}