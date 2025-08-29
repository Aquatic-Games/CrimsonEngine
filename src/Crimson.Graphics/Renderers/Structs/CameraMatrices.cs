using System.Numerics;
using Crimson.Math;

namespace Crimson.Graphics.Renderers.Structs;

/// <summary>
/// A set of camera matrices passed to a shader.
/// </summary>
internal readonly struct CameraMatrices
{
    public const uint SizeInBytes = 144;
    
    /// <summary>
    /// The projection matrix.
    /// </summary>
    public readonly Matrix<float> Projection;
    
    /// <summary>
    /// The view matrix.
    /// </summary>
    public readonly Matrix<float> View;

    /// <summary>
    /// The camera position.
    /// </summary>
    public readonly Vector4T<float> Position;
    
    /// <summary>
    /// Create a new <see cref="CameraMatrices"/>.
    /// </summary>
    /// <param name="projection">The projection matrix.</param>
    /// <param name="view">The view matrix.</param>
    public CameraMatrices(Matrix<float> projection, Matrix<float> view)
    {
        Projection = projection;
        View = view;

        if (Matrix4x4.Invert((Matrix4x4) view, out Matrix4x4 invView))
            Position = new Vector4T<float>((Vector3T<float>) invView.Translation, 0);
    }
}