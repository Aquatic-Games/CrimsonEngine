namespace Crimson.Assets;

public interface IAsset<T>
{
    public static abstract Task<T> LoadAssetAsync(string filePath);
}