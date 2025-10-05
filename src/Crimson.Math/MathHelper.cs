namespace Crimson.Math;

public static class MathHelper
{
    public static uint ToNextPowerOf2(uint value)
    {
        // https://graphics.stanford.edu/%7Eseander/bithacks.html#RoundUpPowerOf2
        value--;
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;
        return ++value;
    }
}