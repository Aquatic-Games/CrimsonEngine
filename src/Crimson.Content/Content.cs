using Crimson.Core;

namespace Crimson.Content;

public static class Content
{
    private static string _contentPathBase;

    private static Dictionary<string, IContentResourceBase> _loadedResources;
    private static Dictionary<string, IDisposable> _disposableResources;

    public static string DirectoryName
    {
        get => Path.GetFileNameWithoutExtension(_contentPathBase);
        set => _contentPathBase = Path.IsPathRooted(value) ? value : Path.Combine(AppContext.BaseDirectory, value);
    }

    static Content()
    {
        _loadedResources = [];
        _disposableResources = [];
        DirectoryName = "Content";
    }

    public static string GetFullyQualifiedName(string name, string extension)
    {
        return Path.Combine(_contentPathBase, Path.ChangeExtension(name, extension));
    }
    
    public static string GetFullyQualifiedName(string name)
    {
        return Path.Combine(_contentPathBase, name);
    }

    public static T Load<T>(string resName) where T : IContentResource<T>
    {
        Logger.Debug($"Loading content resource \"{resName}\".");
        
        string fullPath = Path.IsPathRooted(resName) ? resName : Path.Combine(_contentPathBase, resName);
        bool hasExtension = Path.HasExtension(resName);
        
        Logger.Trace($"  Path: {fullPath}");
        Logger.Trace($"  Has Extension: {hasExtension}");

        T resource;
        if (_loadedResources.TryGetValue(fullPath, out IContentResourceBase res))
            resource = (T) res;
        else
            resource = T.LoadResource(fullPath, hasExtension);
        _loadedResources.Add(fullPath, resource);
        if (resource is IDisposable disposable)
            _disposableResources.Add(fullPath, disposable);

        return resource;
    }

    public static string[] GetContentFiles(string directory, string? searchPattern = null)
    {
        return Directory.GetFiles(GetFullyQualifiedName(directory), searchPattern ?? "*");
    }

    public static void UnloadAllResources()
    {
        Logger.Debug("Unloading all resources.");
        _loadedResources.Clear();

        foreach ((string path, IDisposable resource) in _disposableResources)
        {
            Logger.Trace($"Disposing resource \"{path}\".");
            resource.Dispose();
        }
        
        _disposableResources.Clear();
    }
}