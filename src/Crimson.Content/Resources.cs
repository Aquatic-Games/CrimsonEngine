using System.Reflection;

namespace Crimson.Content;

public static class Resources
{
    public static byte[] LoadEmbeddedResource(string name, Assembly assembly)
    {
        using Stream? stream = assembly.GetManifestResourceStream(name);
        if (stream == null)
            throw new Exception($"Could not find resource stream with name '{name}'");

        using MemoryStream memStream = new MemoryStream();
        stream.CopyTo(memStream);

        return memStream.ToArray();
    }
}