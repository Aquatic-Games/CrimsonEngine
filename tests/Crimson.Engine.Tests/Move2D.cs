using System.Numerics;
using Crimson.Engine.Entities;
using Crimson.Platform;

namespace Crimson.Engine.Tests;

public class Move2D : Component
{
    public override void Update(float dt)
    {
        float moveSpeed = 100 * dt;
        float rotSpeed = 1 * dt;

        if (Input.Input.IsKeyDown(Key.W))
            Transform.Position += Transform.Up * moveSpeed;
        if (Input.Input.IsKeyDown(Key.S))
            Transform.Position += Transform.Down * moveSpeed;

        if (Input.Input.IsKeyDown(Key.A))
            Transform.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitZ, rotSpeed);
        if (Input.Input.IsKeyDown(Key.D))
            Transform.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitZ, -rotSpeed);
    }
}