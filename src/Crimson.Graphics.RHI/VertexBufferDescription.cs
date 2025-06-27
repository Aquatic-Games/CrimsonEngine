namespace Crimson.Graphics.RHI;

public struct VertexBufferDescription
{
    public uint Slot;

    public uint Stride;

    public VertexBufferDescription(uint slot, uint stride)
    {
        Slot = slot;
        Stride = stride;
    }
}