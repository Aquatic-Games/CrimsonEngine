using Crimson.Math;

namespace Crimson.Graphics.RHI;

public struct ColorAttachmentInfo
{
    public Texture Texture;

    public Color ClearColor;

    public LoadOp LoadOp;

    public StoreOp StoreOp;

    public ColorAttachmentInfo(Texture texture, Color clearColor = default, LoadOp loadOp = LoadOp.Clear,
        StoreOp storeOp = StoreOp.Store)
    {
        Texture = texture;
        ClearColor = clearColor;
        LoadOp = loadOp;
        StoreOp = storeOp;
    }
}