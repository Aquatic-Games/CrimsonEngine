using System.Numerics;

namespace Euphoria.Math.Tests;

public class Vector2OperationsTests
{
    [Test]
    public void TestDotProduct()
    {
        Vector2 controlA = new Vector2(1, 2);
        Vector2 controlB = new Vector2(3, 4);
        float controlDot = Vector2.Dot(controlA, controlB);

        Vector2T<float> a = new Vector2T<float>(1, 2);
        Vector2T<float> b = new Vector2T<float>(3, 4);
        float dot = Vector2T.Dot(a, b);
        
        Assert.That(dot, Is.EqualTo(controlDot));
    }

    [Test]
    public void TestMagnitudeSquared()
    {
        Vector2 control = new Vector2(-3, 1.5f);
        float controlMagnitude = control.LengthSquared();
        
        Vector2T<float> vector = new Vector2T<float>(-3, 1.5f);
        float magnitude = vector.LengthSquared();
        
        Assert.That(magnitude, Is.EqualTo(controlMagnitude));
    }
    
    [Test]
    public void TestMagnitude()
    {
        Vector2 control = new Vector2(-3, 1.5f);
        float controlMagnitude = control.Length();
        
        Vector2T<float> vector = new Vector2T<float>(-3, 1.5f);
        float magnitude = vector.Length();
        
        Assert.That(magnitude, Is.EqualTo(controlMagnitude));
    }

    [Test]
    public void TestNormalize()
    {
        Vector2 control = new Vector2(-3, 1.5f);
        Vector2 controlNormalized = Vector2.Normalize(control);

        Vector2T<float> vector = new Vector2T<float>(-3, 1.5f);
        Vector2T<float> normalized = vector.Normalize();
        
        Assert.Multiple(() =>
        {
            Assert.That(normalized.X, Is.EqualTo(controlNormalized.X));
            Assert.That(normalized.Y, Is.EqualTo(controlNormalized.Y));
        });
    }
}