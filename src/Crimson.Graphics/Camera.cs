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

    /*/// <summary>
    /// The skybox to use, if any. If none is provided, the default background color will be used.
    /// </summary>
    public Skybox? Skybox;*/

    internal CameraMatrices Matrices => new CameraMatrices(ProjectionMatrix, ViewMatrix);
}