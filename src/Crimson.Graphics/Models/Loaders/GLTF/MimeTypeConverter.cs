using System.Text.Json;
using System.Text.Json.Serialization;

namespace Crimson.Graphics.Models.Loaders.GLTF;

internal class MimeTypeConverter : JsonConverter<MimeType>
{
    public override MimeType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString()! switch
        {
            "image/jpeg" => MimeType.Jpeg,
            "image/png" => MimeType.Png,
            _ => MimeType.Unknown
        };
    }
    
    public override void Write(Utf8JsonWriter writer, MimeType value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}