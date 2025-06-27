namespace Crimson.Graphics.RHI;

public struct InputElementDescription
{
    public Format Format;

    public uint Offset;

    public uint Location;

    public uint Slot;

    public InputElementDescription(Format format, uint offset, uint location, uint slot)
    {
        Format = format;
        Offset = offset;
        Location = location;
        Slot = slot;
    }
}