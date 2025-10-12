namespace Crimson.Graphics.Models.Loaders.GLTF;

public record struct Buffer
{
    public string? Uri;
    public int ByteLength;
    public string? Name;

    public Buffer(string? uri, int byteLength, string? name)
    {
        Uri = uri;
        ByteLength = byteLength;
        Name = name;
    }
}