using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Crimson.Graphics.Models.Loaders.GLTF;

public record Gltf : IModelLoader<Gltf>
{
    public Asset Asset;
    
    public Accessor[]? Accessors;

    public Buffer[]? Buffers;

    public BufferView[]? BufferViews;

    public Image[]? Images;

    public Gltf(Asset asset, Accessor[]? accessors)
    {
        Asset = asset;
        Accessors = accessors;
    }

    public static Gltf FromPath(string path)
    {
        const uint glbMagic = 0x46546C67;
        const uint json = 0x4E4F534A;
        const uint bin = 0x004E4942;
        Gltf? gltf = null;
        
        using FileStream stream = File.OpenRead(path);
        using BinaryReader reader = new BinaryReader(stream);
        if (reader.ReadUInt32() == glbMagic)
        {
            uint version = reader.ReadUInt32();
            if (version != 2)
                throw new NotSupportedException($"GLB version is {version}, expected 2.");

            reader.ReadUInt32(); // Length

            int chunkSize = reader.ReadInt32();
            uint chunkType = reader.ReadUInt32();

            switch (chunkType)
            {
                case json:
                {
                    byte[] jsonBytes = reader.ReadBytes(chunkSize);
                    string jsonText = Encoding.UTF8.GetString(jsonBytes);
                    Console.WriteLine(jsonText);

                    JsonSerializerOptions options = new()
                    {
                        TypeInfoResolver = GltfSerializerContext.Default,
                        IncludeFields = true,
                        PropertyNameCaseInsensitive = true,
                        Converters = { new AccessorTypeConverter(), new MimeTypeConverter() }
                    };
                    gltf = JsonSerializer.Deserialize<Gltf>(jsonBytes, options);
                    
                    break;
                }
            }
        }
        else
            throw new NotImplementedException();

        return gltf;
    }
}