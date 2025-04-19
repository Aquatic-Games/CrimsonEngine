using System.Runtime.CompilerServices;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace Crimson.Graphics.Utils;

internal static class D3D11Utils
{
    public static unsafe void UpdateBuffer<T>(this ID3D11DeviceContext context, ID3D11Buffer buffer, T data) where T : unmanaged
    {
        MappedSubresource mapped = context.Map(buffer, MapMode.WriteDiscard);
        Unsafe.CopyBlockUnaligned((void*) mapped.DataPointer, Unsafe.AsPointer(ref data), (uint) sizeof(T));
        context.Unmap(buffer);
    }

    public static Format ToD3D(this PixelFormat format, uint size, out uint pitch)
    {
        Format fmt = format switch
        {
            PixelFormat.RGBA8 => Format.R8G8B8A8_UNorm,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };

        pitch = format switch
        {
            PixelFormat.RGBA8 => size * 4,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };

        return fmt;
    }
}