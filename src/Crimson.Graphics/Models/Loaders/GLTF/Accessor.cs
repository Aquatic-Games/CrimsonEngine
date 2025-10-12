namespace Crimson.Graphics.Models.Loaders.GLTF;

public record struct Accessor
{
    public int? BufferView;
    public int ByteOffset;
    public ComponentType ComponentType;
    public bool Normalized;
    public int Count;
    public AccessorType Type;
    public int[]? Max;
    public int[]? Min;
    // TODO: Sparse
    public string? Name;

    public Accessor(int? bufferView, int byteOffset, ComponentType componentType, bool normalized, int count, AccessorType type, int[]? max, int[]? min, string? name)
    {
        BufferView = bufferView;
        ByteOffset = byteOffset;
        ComponentType = componentType;
        Normalized = normalized;
        Count = count;
        Type = type;
        Max = max;
        Min = min;
        Name = name;
    }
}