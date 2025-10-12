using System.Text.Json;
using System.Text.Json.Serialization;

namespace Crimson.Graphics.Models.Loaders.GLTF;

internal class AccessorTypeConverter : JsonConverter<AccessorType>
{
    public override AccessorType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString() switch
        {
            "SCALAR" => AccessorType.Scalar,
            "VEC2" => AccessorType.Vec2,
            "VEC3" => AccessorType.Vec3,
            "VEC4" => AccessorType.Vec4,
            "MAT2" => AccessorType.Mat2,
            "MAT3" => AccessorType.Mat3,
            "MAT4" => AccessorType.Mat4,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    public override void Write(Utf8JsonWriter writer, AccessorType value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}