using System.Numerics;

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
    public readonly Matrix4x4 Projection;
    
    /// <summary>
    /// The view matrix.
    /// </summary>
    public readonly Matrix4x4 View;

    /// <summary>
    /// The camera position.
    /// </summary>
    public readonly Vector4 Position;
    
    /// <summary>
    /// Create a new <see cref="CameraMatrices"/>.
    /// </summary>
    /// <param name="projection">The projection matrix.</param>
    /// <param name="view">The view matrix.</param>
    public CameraMatrices(Matrix4x4 projection, Matrix4x4 view)
    {
        Projection = projection;
        View = view;

        if (Matrix4x4.Invert(view, out Matrix4x4 invView))
            Position = new Vector4(invView.Translation, 0);
    }
}