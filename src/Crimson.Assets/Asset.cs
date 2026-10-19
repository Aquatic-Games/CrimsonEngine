namespace Crimson.Assets;

/// <summary>
/// The asset manager is responsible for loading game content and assets. 
/// </summary>
public static class Asset
{
    /// <summary>
    /// Get/set the base directory containing all assets. Defaults to "Content".
    /// </summary>
    public static string BaseDirectoryName = "Content";

    /// <summary>
    /// Gets the full path to the base directory denoted by <see cref="BaseDirectoryName"/>.
    /// </summary>
    public static string BaseDirectoryPath => Path.Combine(AppContext.BaseDirectory, BaseDirectoryName);
    
    /// <summary>
    /// Load an asset from disk.
    /// </summary>
    /// <param name="path">The path to the asset.</param>
    /// <typeparam name="T">The asset's type.</typeparam>
    /// <returns>The loaded asset.</returns>
    /// <remarks>
    /// This method typically runs synchronously, however it depends on the asset. Some assets will have asynchronous
    /// loading, however all built-in asset types are guaranteed to be thread safe and valid by the time they are used.
    /// </remarks>
    public static T Load<T>(string path) where T : IAsset<T>
    {
        string fullPath = GetAssetPath(path);
        return T.LoadAsset(fullPath);
    }

    /// <summary>
    /// Gets the given asset path as a fully qualified path.
    /// </summary>
    /// <param name="path">The asset's path.</param>
    public static string GetAssetPath(string path)
    {
        if (Path.IsPathFullyQualified(path))
            return path;

        return Path.Combine(BaseDirectoryPath, path);
    }
}