using System.Numerics;
using Crimson.Engine.Entities;
using Crimson.Input;
using Crimson.Platform;

namespace Crimson.Engine.Tests;

public class CameraMove : Component
{
    private Vector2 _rotation;
    
    public override void Update(float dt)
    {
        InputManager input = App.Input;

        const float lookSpeed = 0.01f;

        _rotation.X -= input.MouseDelta.X * lookSpeed;
        _rotation.Y -= input.MouseDelta.Y * lookSpeed;

        _rotation.Y = float.Clamp(_rotation.Y, -float.Pi / 2, float.Pi / 2);

        Transform.Rotation = Quaternion.CreateFromYawPitchRoll(_rotation.X, _rotation.Y, 0);
        
        float moveSpeed = 5 * dt;
        
        if (input.IsKeyDown(Key.W))
            Transform.Position += Transform.Forward * moveSpeed;
        if (input.IsKeyDown(Key.S))
            Transform.Position += Transform.Backward * moveSpeed;
        if (input.IsKeyDown(Key.A))
            Transform.Position += Transform.Left * moveSpeed;
        if (input.IsKeyDown(Key.D))
            Transform.Position += Transform.Right * moveSpeed;
        if (input.IsKeyDown(Key.Space))
            Transform.Position += Transform.Up * moveSpeed;
        if (input.IsKeyDown(Key.LeftControl))
            Transform.Position += Transform.Down * moveSpeed;
        
        
    }
}