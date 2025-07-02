using System.Numerics;

namespace Crimson.Graphics;

public struct Sprite
{
    public Texture Texture;

    public Sprite(Texture texture)
    {
        Texture = texture;
    }
}