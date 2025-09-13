using System.Numerics;
using Crimson.Engine.Entities;
using Crimson.Input;
using Crimson.Math;
using Crimson.Platform;

namespace Crimson.Engine.Tests;

public class CameraMove : Component
{
    private InputAction _moveAction;
    private InputAction _lookAction;
    
    private Vector2T<float> _rotation;

    public override void Initialize()
    {
        ActionSet set = Input.Input.GetActionSet("Main");
        _moveAction = set.GetAction("Move");
        _lookAction = set.GetAction("Look");
    }

    public override void Update(float dt)
    {
        if (_lookAction.Source == InputSource.Absolute)
        {
            const float lookSpeed = 0.01f;
            _rotation -= _lookAction.Value2D * lookSpeed;
        }
        else
        {
            float lookSpeed = 10 * dt;
            _rotation += _lookAction.Value2D * lookSpeed;
        }

        _rotation = new Vector2T<float>(_rotation.X, float.Clamp(_rotation.Y, -float.Pi / 2, float.Pi / 2));

        Transform.Rotation = Quaternion.CreateFromYawPitchRoll(_rotation.X, _rotation.Y, 0);
        
        float moveSpeed = 5 * dt;

        Vector2T<float> moveAxis = _moveAction.Value2D;
        Transform.Position += moveAxis.Y * Transform.Forward * moveSpeed;
        Transform.Position += moveAxis.X * Transform.Right * moveSpeed;
        
        if (Input.Input.IsKeyDown(Key.Space))
            Transform.Position += Transform.Up * moveSpeed;
        if (Input.Input.IsKeyDown(Key.LeftControl))
            Transform.Position += Transform.Down * moveSpeed;
        
        
    }
}