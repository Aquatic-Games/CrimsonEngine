namespace Crimson.Render;

/// <summary>
/// Describes the underlying windowing information for a surface.
/// </summary>
public readonly struct SurfaceInfo(IntPtr nativeHandle)
{
    /// <summary>
    /// The native handle to the surface.
    /// </summary>
    public readonly nint NativeHandle = nativeHandle;
}