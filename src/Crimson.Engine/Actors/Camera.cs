using System.Numerics;
using Crimson.Graphics;

namespace Crimson.Engine.Actors;

public class Camera : Actor
{
    public float FieldOfView;

    public float NearPlane;

    public float FarPlane;

    public Skybox? Skybox;

    public Camera(string name, float fov, float nearPlane = 0.1f, float farPlane = 1000f, Skybox? skybox = null) : base(name)
    {
        FieldOfView = fov;
        NearPlane = nearPlane;
        FarPlane = farPlane;
        Skybox = skybox;
    }

    /// <inheritdoc />
    public override void Loop(float dt)
    {
        base.Loop(dt);

        Graphics.Camera camera = Graphics.Camera.Perspective(Transform.Position, Transform.Forward, Transform.Up,
            FieldOfView, Renderer.Size, NearPlane, FarPlane, Skybox);
        Renderer.AddCamera(in camera);
    }
}