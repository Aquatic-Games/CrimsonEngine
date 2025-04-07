using System.Runtime.CompilerServices;
using Vortice.Direct3D11;

namespace Crimson.Render.Utils;

internal static class D3D11Utils
{
    public static unsafe void UpdateBuffer<T>(this ID3D11DeviceContext context, ID3D11Buffer buffer, T data) where T : unmanaged
    {
        MappedSubresource mapped = context.Map(buffer, MapMode.WriteDiscard);
        Unsafe.CopyBlockUnaligned((void*) mapped.DataPointer, Unsafe.AsPointer(ref data), (uint) sizeof(T));
        context.Unmap(buffer);
    }
}