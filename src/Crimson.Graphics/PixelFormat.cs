namespace Crimson.Graphics;

/// <summary>
/// Represents various pixel formats supported by the GPU.
/// </summary>
public enum PixelFormat
{
    /// <summary>
    /// 8-Bit RGBA.
    /// </summary>
    RGBA8,
    
    /// <summary>
    /// 8-Bit BGRA.
    /// </summary>
    BGRA8,
    
    BC1,
    
    BC1Srgb,
    
    BC2,
    
    BC2Srgb,
    
    BC3,
    
    BC3Srgb,
    
    BC4U,
    
    //BC4S,
    
    BC5U,
    
    //BC5S,
    
    BC6U,
    
    BC6S,
    
    BC7,
    
    BC7Srgb
}