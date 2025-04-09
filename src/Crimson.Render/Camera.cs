using System.Numerics;
using Crimson.Render.Renderers.Structs;

namespace Crimson.Render;

/// <summary>
/// Represents a camera that is used when rendering.
/// </summary>
public struct Camera
{
    /// <summary>
    /// The projection matrix.
    /// </summary>
    public Matrix4x4 ProjectionMatrix;

    /// <summary>
    /// The view matrix.
    /// </summary>
    public Matrix4x4 ViewMatrix;

    internal CameraMatrices Matrices => new CameraMatrices(ProjectionMatrix, ViewMatrix);
}