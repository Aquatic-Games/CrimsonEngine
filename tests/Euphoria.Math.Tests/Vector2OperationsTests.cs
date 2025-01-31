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
    }
}