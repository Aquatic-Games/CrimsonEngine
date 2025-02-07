namespace Euphoria.Math;

public record struct Color
{
    public float R;

    public float G;

    public float B;

    public float A;

    public Color(float r, float g, float b, float a = 1.0f)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public Color(byte r, byte g, byte b, byte a = byte.MaxValue)
    {
        const float multiplier = 1.0f / byte.MaxValue;

        R = r * multiplier;
        G = g * multiplier;
        B = b * multiplier;
        A = a * multiplier;
    }
}