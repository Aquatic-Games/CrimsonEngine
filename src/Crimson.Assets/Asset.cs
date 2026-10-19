namespace Crimson.Assets;

/// <summary>
/// The asset manager is responsible for loading game content and assets. 
/// </summary>
public static class Asset
{
    public static async Task<T> LoadAsync<T>(string path) where T : IAsset<T>
    {
        return await T.LoadAssetAsync(path);
    }
}