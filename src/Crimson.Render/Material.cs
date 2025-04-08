namespace Crimson.Render;

/// <summary>
/// A material that is used during rendering.
/// </summary>
public class Material : IDisposable
{
    public Texture Albedo;

    internal Material(ref readonly MaterialDefinition definition)
    {
        Albedo = definition.Albedo;
    }

    public void Dispose()
    {
        
    }
}