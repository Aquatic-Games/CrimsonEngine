namespace Crimson.Graphics.Models.Loaders;

public interface IModelLoader<T>
{
    public static abstract T FromPath(string path);
}