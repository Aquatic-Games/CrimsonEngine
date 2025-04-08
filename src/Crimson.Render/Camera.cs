using System.Numerics;
using Crimson.Render.Renderers.Structs;

namespace Crimson.Render;

public struct Camera
{
    public Matrix4x4 ProjectionMatrix;

    public Matrix4x4 ViewMatrix;

    internal CameraMatrices Matrices => new CameraMatrices(ProjectionMatrix, ViewMatrix);
}