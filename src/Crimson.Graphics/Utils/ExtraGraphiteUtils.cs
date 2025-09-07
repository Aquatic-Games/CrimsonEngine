using Crimson.Math;
using Graphite;
using Graphite.Core;

namespace Crimson.Graphics.Utils;

internal static class ExtraGraphiteUtils
{
    public static Size2D ToGraphite(this Size<int> size)
        => new Size2D((uint) size.Width, (uint) size.Height);

    public static Format ToGraphite(this PixelFormat format)
    {
        return format switch
        {
            PixelFormat.RGBA8 => Format.R8G8B8A8_UNorm,
            PixelFormat.BGRA8 => Format.B8G8R8A8_UNorm,
            PixelFormat.BC1 => Format.BC1_UNorm,
            PixelFormat.BC1Srgb => Format.BC1_UNorm_SRGB,
            PixelFormat.BC2 => Format.BC2_UNorm,
            PixelFormat.BC2Srgb => Format.BC2_UNorm_SRGB,
            PixelFormat.BC3 => Format.BC3_UNorm,
            PixelFormat.BC3Srgb => Format.BC3_UNorm_SRGB,
            PixelFormat.BC4U => Format.BC4_UNorm,
            PixelFormat.BC5U => Format.BC5_UNorm,
            PixelFormat.BC6U => Format.BC6H_UF16,
            PixelFormat.BC6S => Format.BC6H_SF16,
            PixelFormat.BC7 => Format.BC7_UNorm,
            PixelFormat.BC7Srgb => Format.BC7_UNorm_SRGB,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }
}