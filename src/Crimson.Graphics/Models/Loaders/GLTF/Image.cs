namespace Crimson.Graphics.Models.Loaders.GLTF;

public record struct Image
{
    public string? Uri;
    public MimeType? MimeType;
    public int? BufferView;
    public string? Name;

    public Image(string? uri, MimeType? mimeType, int? bufferView, string? name)
    {
        Uri = uri;
        MimeType = mimeType;
        BufferView = bufferView;
        Name = name;
    }
}