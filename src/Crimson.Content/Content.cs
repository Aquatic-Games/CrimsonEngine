using Crimson.Core;

namespace Crimson.Content;

public static class Content
{
    private static string _contentPathBase;

    private static Dictionary<string, IContentResourceBase> _loadedResources;
    private static Dictionary<string, IDisposable> _persistentResources;
    private static Dictionary<string, IDisposable> _perSceneResources;

    public static string DirectoryName
    {
        get => Path.GetFileNameWithoutExtension(_contentPathBase);
        set => _contentPathBase = Path.IsPathRooted(value) ? value : Path.Combine(AppContext.BaseDirectory, value);
    }

    static Content()
    {
        _loadedResources = [];
        _persistentResources = [];
        _perSceneResources = [];
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

    public static T Load<T>(string resName, bool persistent = false) where T : IContentResource<T>
    {
        string fullPath = Path.IsPathRooted(resName) ? resName : Path.Combine(_contentPathBase, resName);
        bool hasExtension = Path.HasExtension(resName);

        T resource;
        if (_loadedResources.TryGetValue(fullPath, out IContentResourceBase res))
            resource = (T) res;
        else
        {
            Logger.Debug($"Loading content resource \"{resName}\".");
            Logger.Trace($"  Path: {fullPath}");
            Logger.Trace($"  Has Extension: {hasExtension}");
            
            resource = T.LoadResource(fullPath, hasExtension);
            _loadedResources.Add(fullPath, resource);
            if (resource is IDisposable disposable)
            {
                if (persistent)
                    _persistentResources.Add(fullPath, disposable);
                else
                    _perSceneResources.Add(fullPath, disposable);
            }
        }

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

        foreach ((string path, IDisposable resource) in _perSceneResources)
        {
            Logger.Trace($"Disposing resource \"{path}\".");
            resource.Dispose();
        }
        
        _perSceneResources.Clear();
    }

    public static void CleanUpEverything()
    {
        Logger.Debug("Cleaning up content.");
        UnloadAllResources();
        
        foreach ((string path, IDisposable resource) in _persistentResources)
        {
            Logger.Trace($"Disposing persistent resource \"{path}\".");
            resource.Dispose();
        }
        
        _persistentResources.Clear();
    }
}