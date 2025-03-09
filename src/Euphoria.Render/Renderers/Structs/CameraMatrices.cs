using System.Numerics;

namespace Euphoria.Render.Renderers.Structs;

/// <summary>
/// A set of camera matrices passed to a shader.
/// </summary>
public struct CameraMatrices
{
    public const uint SizeInBytes = 128;
    
    /// <summary>
    /// The projection matrix.
    /// </summary>
    public Matrix4x4 Projection;
    
    /// <summary>
    /// The view matrix.
    /// </summary>
    public Matrix4x4 View;

    /// <summary>
    /// Create a new <see cref="CameraMatrices"/>.
    /// </summary>
    /// <param name="projection">The projection matrix.</param>
    /// <param name="view">The view matrix.</param>
    public CameraMatrices(Matrix4x4 projection, Matrix4x4 view)
    {
        Projection = projection;
        View = view;
    }
}