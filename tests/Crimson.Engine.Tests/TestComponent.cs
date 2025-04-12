using System.Numerics;
using Crimson.Engine.Entities;

namespace Crimson.Engine.Tests;

public class TestComponent : Component
{
    public override void Update(float dt)
    {
        Transform.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, dt) *
                              Quaternion.CreateFromAxisAngle(Vector3.UnitX, dt * 0.2f) *
                              Quaternion.CreateFromAxisAngle(Vector3.UnitZ, dt * 0.7f);
    }
}