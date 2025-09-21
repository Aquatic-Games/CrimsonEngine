using System.Numerics;

namespace Crimson.Graphics.Renderers.Structs;

internal readonly struct DrawCall
{
    public readonly Renderable Renderable;

    public readonly Matrix4x4 WorldMatrix;

    public readonly MaterialProperties MaterialProperties;

    public DrawCall(Renderable renderable, Matrix4x4 worldMatrix)
    {
        Renderable = renderable;
        WorldMatrix = worldMatrix;
        MaterialProperties = MaterialProperties.FromMaterial(Renderable.Material);
    }
}