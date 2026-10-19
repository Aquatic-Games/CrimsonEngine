namespace Crimson.Assets;

public interface IAsset<out T>
{
    public static abstract T LoadAsset(string filePath);
}