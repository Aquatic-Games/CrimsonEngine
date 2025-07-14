using Crimson.Graphics.Renderers.Structs;
using Crimson.Math;
using SDL3;

namespace Crimson.Graphics.Renderers;

internal class SpriteRenderer : IDisposable
{
    private readonly TextureBatcher _batcher;

    public SpriteRenderer(IntPtr device, SDL.GPUTextureFormat swapchainFormat)
    {
        _batcher = new TextureBatcher(device, swapchainFormat);
    }

    public void DrawSprite(ref readonly Sprite sprite, Matrix<float> matrix)
    {
        Size<int> halfSize = sprite.Texture.Size / 2;
        
        Vector2T<float> topLeft = new Vector2T<float>(-halfSize.Width, halfSize.Height) * matrix;
        Vector2T<float> topRight = new Vector2T<float>(halfSize.Width, halfSize.Height) * matrix;
        Vector2T<float> bottomLeft = new Vector2T<float>(-halfSize.Width, -halfSize.Height) * matrix;
        Vector2T<float> bottomRight = new Vector2T<float>(halfSize.Width, -halfSize.Height) * matrix;

        _batcher.AddToDrawQueue(new TextureBatcher.Draw(sprite.Texture, topLeft, topRight, bottomLeft, bottomRight,
            new Rectangle<int>(Vector2T<int>.Zero, sprite.Texture.Size), Color.White, BlendMode.Blend));
    }

    public bool Render(IntPtr cb, IntPtr swapchainTarget, bool shouldClear, Size<int> swapchainSize, CameraMatrices matrices)
    {
        return _batcher.Render(cb, swapchainTarget, shouldClear, swapchainSize, matrices);
    }
    
    public void Dispose()
    {
        _batcher.Dispose();
    }
}