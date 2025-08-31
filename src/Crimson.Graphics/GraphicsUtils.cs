namespace Crimson.Graphics;

public static class GraphicsUtils
{
    public static bool IsCompressed(this PixelFormat format)
    {
        switch (format)
        {
            case PixelFormat.RGBA8:
            case PixelFormat.BGRA8:
                return false;
            
            case PixelFormat.BC1:
            case PixelFormat.BC1Srgb:
            case PixelFormat.BC2:
            case PixelFormat.BC2Srgb:
            case PixelFormat.BC3:
            case PixelFormat.BC3Srgb:
            case PixelFormat.BC4U:
            case PixelFormat.BC5U:
            case PixelFormat.BC6U:
            case PixelFormat.BC6S:
            case PixelFormat.BC7:
            case PixelFormat.BC7Srgb:
                return true;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }
}