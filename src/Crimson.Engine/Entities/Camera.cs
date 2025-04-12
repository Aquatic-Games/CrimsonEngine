using System.Numerics;
using Crimson.Math;
using Crimson.Render;

namespace Crimson.Engine.Entities;

public class Camera : Entity
{
    private float _fov;
    private float _near;
    private float _far;

    public float FieldOfView
    {
        get => float.RadiansToDegrees(_fov);
        set => _fov = float.DegreesToRadians(value);
    }

    public ref float NearPlane => ref _near;

    public ref float FarPlane => ref _far;
    
    public Camera(string name, float fieldOfView, float near, float far) : base(name)
    {
        _fov = float.DegreesToRadians(fieldOfView);
        _near = near;
        _far = far;
    }

    public override void Draw()
    {
        Graphics graphics = App.Graphics;
        Size<int> size = graphics.RenderSize;

        graphics.Camera = new Render.Camera()
        {
            ProjectionMatrix =
                Matrix4x4.CreatePerspectiveFieldOfView(_fov, size.Width / (float) size.Height, _near, _far),
            ViewMatrix =
                Matrix4x4.CreateLookAt(Transform.Position, Transform.Position + Transform.Forward, Transform.Up)
        };
    }
}