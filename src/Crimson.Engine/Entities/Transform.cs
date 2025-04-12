using System.Numerics;

namespace Crimson.Engine.Entities;

public struct Transform
{
    public Vector3 Position;

    public Quaternion Rotation;

    public Vector3 Scale;

    public Vector3 Origin;

    public Transform()
    {
        Position = Vector3.Zero;
        Rotation = Quaternion.Identity;
        Scale = Vector3.One;
        Origin = Vector3.Zero;
    }

    public Transform(Vector3 position) : this()
    {
        Position = position;
    }

    public Transform(Vector3 position, Quaternion rotation) : this()
    {
        Position = position;
        Rotation = rotation;
    }

    public Vector3 Forward => Vector3.Transform(-Vector3.UnitZ, Rotation);

    public Vector3 Backward => Vector3.Transform(Vector3.UnitZ, Rotation);

    public Vector3 Up => Vector3.Transform(Vector3.UnitY, Rotation);

    public Vector3 Down => Vector3.Transform(-Vector3.UnitY, Rotation);

    public Vector3 Right => Vector3.Transform(Vector3.UnitX, Rotation);

    public Vector3 Left => Vector3.Transform(-Vector3.UnitX, Rotation);

    public Matrix4x4 WorldMatrix => Matrix4x4.CreateScale(Scale) *
                                    Matrix4x4.CreateTranslation(-Origin) *
                                    Matrix4x4.CreateFromQuaternion(Rotation) *
                                    Matrix4x4.CreateTranslation(Position);
}