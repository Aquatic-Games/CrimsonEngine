namespace Crimson.Render;

public readonly struct SurfaceInfo
{
    public readonly nint NativeHandle;

    public SurfaceInfo(IntPtr nativeHandle)
    {
        NativeHandle = nativeHandle;
    }
}