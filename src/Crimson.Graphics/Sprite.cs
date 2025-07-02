using System.Numerics;

namespace Crimson.Graphics;

public struct Sprite
{
    public Texture Texture;

    public Matrix3x2 Matrix;

    public Sprite(Texture texture, Matrix3x2 matrix)
    {
        Texture = texture;
        Matrix = matrix;
    }
}