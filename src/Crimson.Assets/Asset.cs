namespace Crimson.Assets;

/// <summary>
/// The asset manager is responsible for loading game content and assets. 
/// </summary>
public static class Asset
{
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
        return T.LoadAsset(path);
    }
}