using System.Numerics;
using Crimson.Graphics.Renderers.Structs;

namespace Crimson.Graphics;

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