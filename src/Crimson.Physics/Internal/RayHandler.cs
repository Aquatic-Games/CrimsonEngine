using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.Trees;

namespace Crimson.Physics.Internal;

internal struct RayHandler : IRayHitHandler
{
    public bool HasHit;
    
    public float RayDistance;

    public CollidableReference Collidable;

    public Vector3 Normal;

    public int ChildIndex;

    public RayHandler()
    {
        RayDistance = float.MaxValue;
    }
    
    public bool AllowTest(CollidableReference collidable)
    {
        return true;
    }
    
    public bool AllowTest(CollidableReference collidable, int childIndex)
    {
        return true;
    }

    public void OnRayHit(in RayData ray, ref float maximumT, float t, Vector3 normal, CollidableReference collidable,
        int childIndex)
    {
        HasHit = true;

        if (t < RayDistance)
        {
            RayDistance = t;
            Collidable = collidable;
            Normal = normal;
            ChildIndex = childIndex;
        }
    }
}