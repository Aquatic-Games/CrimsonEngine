using System.Numerics;
using Crimson.Graphics;
using Crimson.Math;

namespace Crimson.Engine.Entities.Components;

public class Sprite : Component
{
    public Texture Texture;
    
    public Sprite(Texture texture)
    {
        Texture = texture;
    }

    public override void Draw()
    {
        // mmm I love conversions
        // TODO: Add the rest of the matrix stuff then move everything over to Crimson.Math.
        Matrix4x4 transMatrix = Entity.WorldMatrix;
        Matrix<float> matrix = new Matrix<float>(
            transMatrix.M11, transMatrix.M12, transMatrix.M13, transMatrix.M14,
            transMatrix.M21, transMatrix.M22, transMatrix.M23, transMatrix.M24,
            transMatrix.M31, transMatrix.M32, transMatrix.M33, transMatrix.M34,
            transMatrix.M41, transMatrix.M42, transMatrix.M43, transMatrix.M44
        );
        
        Renderer.DrawSprite(new Graphics.Sprite(Texture), matrix);
    }
}