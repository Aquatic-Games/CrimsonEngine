namespace Crimson.Graphics.Models.Loaders.GLTF;

public record struct BufferView
{
    public int Buffer;
    public int ByteOffset;
    public int ByteLength;
    public int? ByteStride;
    public BufferTarget? Target;
    public string? Name;

    public BufferView(int buffer, int byteOffset, int byteLength, int? byteStride, BufferTarget? target, string? name)
    {
        Buffer = buffer;
        ByteOffset = byteOffset;
        ByteLength = byteLength;
        ByteStride = byteStride;
        Target = target;
        Name = name;
    }
}