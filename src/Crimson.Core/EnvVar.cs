namespace Crimson.Core;

public static class EnvVar
{
    public const string Fullscreen = "CRIMSON_FULLSCREEN";

    public const string SurfaceDisplay = "CRIMSON_SURFACE_DISPLAY";

    public const string ForceVulkan = "CRIMSON_FORCE_VULKAN";

    public static bool IsSet(string name)
        => Environment.GetEnvironmentVariable(name) != null;
    
    public static bool IsTrue(string name)
        => Environment.GetEnvironmentVariable(name) is "true" or "1";

    public static bool TryGetValue(string name, out string? value)
    {
        value = Environment.GetEnvironmentVariable(name);
        return value != null;
    }

    public static bool TryGetInt(string name, out int value)
    {
        if (!TryGetValue(name, out string s))
        {
            value = 0;
            return false;
        }

        return int.TryParse(s, out value);
    }
}