using System.Diagnostics.CodeAnalysis;
using Crimson.Core;

namespace Crimson.Content;

public static class Content
{
    private static string _contentPathBase;

    private static Dictionary<string, IContentResourceBase> _persistentResources;
    private static Dictionary<string, IContentResourceBase> _perSceneResources;
    private static Dictionary<string, IDisposable> _persistentDisposables;
    private static Dictionary<string, IDisposable> _perSceneDisposables;

    public static string DirectoryName
    {
        get => Path.GetFileNameWithoutExtension(_contentPathBase);
        set => _contentPathBase = Path.IsPathRooted(value) ? value : Path.Combine(AppContext.BaseDirectory, value);
    }

    static Content()
    {
        _persistentResources = [];
        _perSceneResources = [];
        _persistentDisposables = [];
        _perSceneDisposables = [];
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

    public static bool FileExists(string name)
    {
        return Path.Exists(GetFullyQualifiedName(name));
    }

    public static T Load<T>(string resName, bool persistent = false) where T : IContentResource<T>
    {
        string fullPath = Path.IsPathRooted(resName) ? resName : Path.Combine(_contentPathBase, resName);
        bool hasExtension = Path.HasExtension(resName);

        T resource;
        IContentResourceBase res;
        if (_perSceneResources.TryGetValue(fullPath, out res) || _persistentResources.TryGetValue(fullPath, out res))
            resource = (T) res;
        else
        {
            Logger.Debug($"Loading content resource \"{resName}\".");
            Logger.Trace($"  Path: {fullPath}");
            Logger.Trace($"  Has Extension: {hasExtension}");
            
            resource = T.LoadResource(fullPath, hasExtension);
            if (persistent)
                _persistentResources.Add(fullPath, resource);
            else
                _perSceneResources.Add(fullPath, resource);
            
            if (resource is IDisposable disposable)
            {
                if (persistent)
                    _persistentDisposables.Add(fullPath, disposable);
                else
                    _perSceneDisposables.Add(fullPath, disposable);
            }
        }

        return resource;
    }

    public static bool TryLoad<T>(string resName, [NotNullWhen(true)] out T? resource, bool persistent = false) where T : IContentResource<T>
    {
        try
        {
            resource = Load<T>(resName, persistent);
            return true;
        }
        catch (Exception)
        {
            resource = default;
            return false;
        }
    }

    public static string[] GetContentFiles(string directory, string? searchPattern = null)
    {
        return Directory.GetFiles(GetFullyQualifiedName(directory), searchPattern ?? "*");
    }

    public static void UnloadPerSceneResources()
    {
        Logger.Debug("Unloading per-scene resources.");
        _perSceneResources.Clear();

        foreach ((string path, IDisposable resource) in _perSceneDisposables)
        {
            Logger.Trace($"Disposing resource \"{path}\".");
            resource.Dispose();
        }
        
        _perSceneDisposables.Clear();
    }

    public static void UnloadAllResources()
    {
        Logger.Debug("Unloading all resources.");
        UnloadPerSceneResources();
        _persistentResources.Clear();
        
        foreach ((string path, IDisposable resource) in _persistentDisposables)
        {
            Logger.Trace($"Disposing persistent resource \"{path}\".");
            resource.Dispose();
        }
        
        _persistentDisposables.Clear();
    }
}