using System.Text.Json.Serialization;

namespace Crimson.Graphics.Models.Loaders.GLTF;

[JsonSerializable(typeof(Gltf))]
internal partial class GltfSerializerContext : JsonSerializerContext;