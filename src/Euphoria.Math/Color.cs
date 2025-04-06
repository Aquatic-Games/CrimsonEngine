namespace Euphoria.Math;

/// <summary>
/// An RGBA floating-point color.
/// </summary>
public record struct Color
{
    /// <summary>
    /// The red component.
    /// </summary>
    public float R;

    /// <summary>
    /// The green component.
    /// </summary>
    public float G;

    /// <summary>
    /// The blue component.
    /// </summary>
    public float B;

    /// <summary>
    /// The alpha component.
    /// </summary>
    public float A;

    /// <summary>
    /// Create a <see cref="Color"/> from floating-point values.
    /// </summary>
    /// <param name="r">The red component.</param>
    /// <param name="g">The green component.</param>
    /// <param name="b">The blue component.</param>
    /// <param name="a">The alpha component.</param>
    public Color(float r, float g, float b, float a = 1.0f)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    /// <summary>
    /// Create a <see cref="Color"/> from byte values.
    /// </summary>
    /// <param name="r">The red component.</param>
    /// <param name="g">The green component.</param>
    /// <param name="b">The blue component.</param>
    /// <param name="a">The alpha component.</param>
    public Color(byte r, byte g, byte b, byte a = byte.MaxValue)
    {
        const float multiplier = 1.0f / 255.0f;

        R = r * multiplier;
        G = g * multiplier;
        B = b * multiplier;
        A = a * multiplier;
    }

    /// <summary>
    /// Create a <see cref="Color"/> from a packed 32-bit RGBA value.
    /// </summary>
    /// <param name="packedRgba">The 32-bit RGBA value.</param>
    public Color(uint packedRgba) : this((byte) (packedRgba >> 24) & 0xFF, (byte) (packedRgba >> 16) & 0xFF,
        (byte) (packedRgba >> 8) & 0xFF, (byte) (packedRgba & 0xFF)) { }
}