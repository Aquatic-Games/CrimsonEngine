using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace Crimson.Graphics.Models.Loaders.GLTF;

public class Gltf : IModelLoader<Gltf>
{
    public Asset Asset;
    
    public Accessor[]? Accessors;

    public Gltf(Asset asset, Accessor[]? accessors)
    {
        Asset = asset;
        Accessors = accessors;
    }

    public static unsafe Gltf FromPath(string path)
    {
        const uint glbMagic = 0x46546C67;
        Gltf gltf;
        
        using FileStream stream = File.OpenRead(path);
        using BinaryReader reader = new BinaryReader(stream);
        if (reader.ReadUInt32() == glbMagic)
        {
            uint version = reader.ReadUInt32();
            if (version != 2)
                throw new NotSupportedException($"GLB version is {version}, expected 2.");

            reader.ReadUInt32(); // Length;
            
            
        }

        throw new NotImplementedException();

        return gltf;
    }
}