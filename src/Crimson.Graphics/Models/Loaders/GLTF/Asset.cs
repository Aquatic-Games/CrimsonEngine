namespace Crimson.Graphics.Models.Loaders.GLTF;

public record struct Asset
{
    public string? Copyright;
    public string? Generator;
    public string Version;
    public string? MinVersion;

    public Asset(string? copyright, string? generator, string version, string? minVersion)
    {
        Copyright = copyright;
        Generator = generator;
        Version = version;
        MinVersion = minVersion;
    }
}