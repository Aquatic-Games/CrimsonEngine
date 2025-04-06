using System.Numerics;

namespace Euphoria.Render.Renderers.Structs;

internal struct WorldRenderable
{
    public readonly Renderable Renderable;

    public readonly Matrix4x4 WorldMatrix;

    public WorldRenderable(Renderable renderable, Matrix4x4 worldMatrix)
    {
        Renderable = renderable;
        WorldMatrix = worldMatrix;
    }
}