namespace Crimson.Graphics.RHI;

public struct TextureInfo
{
    public TextureType Type;

    public uint Width;

    public uint Height;

    public Format Format;

    public TextureUsage Usage;

    public TextureInfo(TextureType type, uint width, uint height, Format format, TextureUsage usage)
    {
        Type = type;
        Width = width;
        Height = height;
        Format = format;
        Usage = usage;
    }
}