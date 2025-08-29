using Crimson.Graphics.Renderers.Structs;
using Crimson.Math;

namespace Crimson.Graphics;

/// <summary>
/// Represents a camera that is used when rendering.
/// </summary>
public struct Camera
{
    /// <summary>
    /// The projection matrix.
    /// </summary>
    public Matrix<float> ProjectionMatrix;

    /// <summary>
    /// The view matrix.
    /// </summary>
    public Matrix<float> ViewMatrix;

    /*/// <summary>
    /// The skybox to use, if any. If none is provided, the default background color will be used.
    /// </summary>*/
    // TODO: Readd public Skybox? Skybox;

    internal CameraMatrices Matrices => new CameraMatrices(ProjectionMatrix, ViewMatrix);
}