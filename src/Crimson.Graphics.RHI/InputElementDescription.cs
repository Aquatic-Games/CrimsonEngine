namespace Crimson.Graphics.RHI;

public struct InputElementDescription
{
    public Semantic Semantic;
    
    public Format Format;

    public uint Offset;

    public uint Location;

    public uint Slot;

    public InputElementDescription(Semantic semantic, Format format, uint offset, uint location, uint slot)
    {
        Semantic = semantic;
        Format = format;
        Offset = offset;
        Location = location;
        Slot = slot;
    }
}