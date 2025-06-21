using System.Numerics;
using Crimson.Graphics;
using Crimson.Math;

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

    public Skybox? Skybox;
    
    public Camera(string name, float fieldOfView, float near, float far) : base(name)
    {
        _fov = float.DegreesToRadians(fieldOfView);
        _near = near;
        _far = far;
    }

    public override void Draw()
    {
        Size<int> size = Renderer.RenderSize;

        Renderer.Camera = new Graphics.Camera()
        {
            ProjectionMatrix =
                Matrix4x4.CreatePerspectiveFieldOfView(_fov, size.Width / (float) size.Height, _near, _far),
            ViewMatrix =
                Matrix4x4.CreateLookAt(Transform.Position, Transform.Position + Transform.Forward, Transform.Up),
            Skybox = Skybox
        };
    }
}