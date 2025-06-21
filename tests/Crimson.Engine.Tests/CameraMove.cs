using System.Numerics;
using Crimson.Engine.Entities;
using Crimson.Platform;

namespace Crimson.Engine.Tests;

public class CameraMove : Component
{
    private Vector2 _rotation;
    
    public override void Update(float dt)
    {
        const float lookSpeed = 0.01f;

        _rotation.X -= Input.Input.MouseDelta.X * lookSpeed;
        _rotation.Y -= Input.Input.MouseDelta.Y * lookSpeed;

        _rotation.Y = float.Clamp(_rotation.Y, -float.Pi / 2, float.Pi / 2);

        Transform.Rotation = Quaternion.CreateFromYawPitchRoll(_rotation.X, _rotation.Y, 0);
        
        float moveSpeed = 5 * dt;
        
        if (Input.Input.IsKeyDown(Key.W))
            Transform.Position += Transform.Forward * moveSpeed;
        if (Input.Input.IsKeyDown(Key.S))
            Transform.Position += Transform.Backward * moveSpeed;
        if (Input.Input.IsKeyDown(Key.A))
            Transform.Position += Transform.Left * moveSpeed;
        if (Input.Input.IsKeyDown(Key.D))
            Transform.Position += Transform.Right * moveSpeed;
        if (Input.Input.IsKeyDown(Key.Space))
            Transform.Position += Transform.Up * moveSpeed;
        if (Input.Input.IsKeyDown(Key.LeftControl))
            Transform.Position += Transform.Down * moveSpeed;
        
        
    }
}