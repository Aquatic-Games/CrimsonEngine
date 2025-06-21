namespace Crimson.Content;

public interface IContentResource<T> : IContentResourceBase
{
    public static abstract T LoadResource(string fullPath, bool hasExtension);
}