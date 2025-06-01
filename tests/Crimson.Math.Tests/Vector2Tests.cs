using System.Numerics;

namespace Crimson.Math.Tests;

public class Vector2Tests
{
    #region Struct
    
    [Test]
    public void TestConstruct()
    {
        Vector2T<int> vector = new Vector2T<int>(1, 2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vector.X, Is.EqualTo(1));
            Assert.That(vector.Y, Is.EqualTo(2));
        }
    }

    [Test]
    public void TestConstructScalar()
    {
        Vector2T<int> vector = new Vector2T<int>(2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vector.X, Is.EqualTo(2));
            Assert.That(vector.Y, Is.EqualTo(2));
        }
    }

    [Test]
    public void TestZero()
    {
        Vector2T<int> vector = Vector2T<int>.Zero;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vector.X, Is.EqualTo(0));
            Assert.That(vector.Y, Is.EqualTo(0));
        }
    }

    [Test]
    public void TestOne()
    {
        Vector2T<int> vector = Vector2T<int>.One;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vector.X, Is.EqualTo(1));
            Assert.That(vector.Y, Is.EqualTo(1));
        }
    }

    [Test]
    public void TestUnitX()
    {
        Vector2T<int> vector = Vector2T<int>.UnitX;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vector.X, Is.EqualTo(1));
            Assert.That(vector.Y, Is.EqualTo(0));
        }
    }
    
    [Test]
    public void TestUnitY()
    {
        Vector2T<int> vector = Vector2T<int>.UnitY;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vector.X, Is.EqualTo(0));
            Assert.That(vector.Y, Is.EqualTo(1));
        }
    }

    [Test]
    public void TestIndexer()
    {
        Vector2T<int> vector = new Vector2T<int>(1, 2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vector[0], Is.EqualTo(1));
            Assert.That(vector[1], Is.EqualTo(2));
        }
    }

    [Test]
    public void TestAs()
    {
        Vector2T<float> floatVector = new Vector2T<float>(3.7f, -2.3f);
        Vector2T<int> intVector = floatVector.As<int>();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(intVector.X, Is.EqualTo(3));
            Assert.That(intVector.Y, Is.EqualTo(-2));
        }
    }

    [Test]
    public void TestVectorVectorAddition()
    {
        Vector2T<int> a = new Vector2T<int>(1, 2);
        Vector2T<int> b = new Vector2T<int>(5, 6);

        Vector2T<int> c = a + b;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.X, Is.EqualTo(1 + 5));
            Assert.That(c.Y, Is.EqualTo(2 + 6));
        }
    }
    
    [Test]
    public void TestVectorVectorSubtraction()
    {
        Vector2T<int> a = new Vector2T<int>(1, 2);
        Vector2T<int> b = new Vector2T<int>(5, 6);

        Vector2T<int> c = a - b;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.X, Is.EqualTo(1 - 5));
            Assert.That(c.Y, Is.EqualTo(2 - 6));
        }
    }
    
    [Test]
    public void TestVectorVectorMultiplication()
    {
        Vector2T<int> a = new Vector2T<int>(1, 2);
        Vector2T<int> b = new Vector2T<int>(5, 6);

        Vector2T<int> c = a * b;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.X, Is.EqualTo(1 * 5));
            Assert.That(c.Y, Is.EqualTo(2 * 6));
        }
    }
    
    [Test]
    public void TestVectorMatrixMultiplication()
    {
        Vector2T<float> control = Vector2T.Transform(new Vector2T<float>(5, 10), Matrix.RotateZ<float>(3));
        Vector2T<float> test = new Vector2T<float>(5, 10) * Matrix.RotateZ<float>(3);
        
        Assert.That(test, Is.EqualTo(control));
    }
    
    [Test]
    public void TestVectorScalarMultiplication()
    {
        Vector2T<int> a = new Vector2T<int>(1, 2);
        const int b = 4;

        Vector2T<int> c = a * b;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.X, Is.EqualTo(1 * 4));
            Assert.That(c.Y, Is.EqualTo(2 * 4));
        }
    }
    
    [Test]
    public void TestVectorVectorDivision()
    {
        Vector2T<float> a = new Vector2T<float>(1, 2);
        Vector2T<float> b = new Vector2T<float>(5, 6);

        Vector2T<float> c = a / b;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.X, Is.EqualTo(1.0f / 5.0f));
            Assert.That(c.Y, Is.EqualTo(2.0f / 6.0f));
        }
    }
    
    [Test]
    public void TestVectorScalarDivision()
    {
        Vector2T<float> a = new Vector2T<float>(1, 2);
        const float b = 4;

        Vector2T<float> c = a / b;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.X, Is.EqualTo(1.0f / 4.0f));
            Assert.That(c.Y, Is.EqualTo(2.0f / 4.0f));
        }
    }

    [Test]
    public void TestEquals()
    {
        Vector2T<int> a = new Vector2T<int>(1, 2);
        Vector2T<int> b = new Vector2T<int>(4, 3);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(a == a, Is.True);
            Assert.That(a == b, Is.False);
        }
    }

    [Test]
    public void TestNotEquals()
    {
        Vector2T<int> a = new Vector2T<int>(1, 2);
        Vector2T<int> b = new Vector2T<int>(4, 3);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(a != a, Is.False);
            Assert.That(a != b, Is.True);
        }
    }

    [Test]
    public void TestToString()
    {
        Vector2T<int> vector = new Vector2T<int>(1, 2);
        Assert.That(vector.ToString(), Is.EqualTo("X: 1, Y: 2"));
    }
    
    #endregion

    #region Operations

    [Test]
    public void TestDot()
    {
        float control = Vector2.Dot(new Vector2(1, 2), new Vector2(4, 3));
        float test = Vector2T.Dot(new Vector2T<float>(1, 2), new Vector2T<float>(4, 3));
        
        Assert.That(test, Is.EqualTo(control));
    }

    [Test]
    public void TestMagnitudeSquared()
    {
        float control = new Vector2(1, 2).LengthSquared();
        float test = Vector2T.MagnitudeSquared(new Vector2T<float>(1, 2));
        
        Assert.That(test, Is.EqualTo(control));
    }

    [Test]
    public void TestMagnitude()
    {
        float control = new Vector2(1, 2).Length();
        float test = Vector2T.Magnitude(new Vector2T<float>(1, 2));
        
        Assert.That(test, Is.EqualTo(control));
    }
    
    [Test]
    public void TestDistanceSquared()
    {
        Vector2 controlA = new Vector2(1, 2);
        Vector2 controlB = new Vector2(5, 6);
        float control = Vector2.DistanceSquared(controlA, controlB);

        Vector2T<float> testA = new Vector2T<float>(1, 2);
        Vector2T<float> testB = new Vector2T<float>(5, 6);
        float test = Vector2T.DistanceSquared(testA, testB);
        
        Assert.That(test, Is.EqualTo(control));
    }
    
    [Test]
    public void TestDistance()
    {
        Vector2 controlA = new Vector2(1, 2);
        Vector2 controlB = new Vector2(5, 6);
        float control = Vector2.Distance(controlA, controlB);

        Vector2T<float> testA = new Vector2T<float>(1, 2);
        Vector2T<float> testB = new Vector2T<float>(5, 6);
        float test = Vector2T.Distance(testA, testB);
        
        Assert.That(test, Is.EqualTo(control));
    }

    [Test]
    public void TestTransform()
    {
        Vector2 control = Vector2.Transform(new Vector2(5, 10), Matrix4x4.CreateRotationZ(3));
        Vector2T<float> test = Vector2T.Transform(new Vector2T<float>(5, 10), Matrix.RotateZ<float>(3));

        using (Assert.EnterMultipleScope())
        {
            // System.Numerics has a different impl meaning its output is very slightly different, but only by a few
            // decimal points. Cast to int to remove this difference while being a "good enough" test.
            Assert.That((int) test.X, Is.EqualTo((int) control.X));
            Assert.That((int) test.Y, Is.EqualTo((int) control.Y));
        }
    }
    
    [Test]
    public void TestTransformNormal()
    {
        Vector2 control = Vector2.TransformNormal(new Vector2(5, 10), Matrix4x4.CreateRotationZ(3));
        Vector2T<float> test = Vector2T.TransformNormal(new Vector2T<float>(5, 10), Matrix.RotateZ<float>(3));

        using (Assert.EnterMultipleScope())
        {
            // System.Numerics has a different impl meaning its output is very slightly different, but only by a few
            // decimal points. Cast to int to remove this difference while being a "good enough" test.
            Assert.That((int) test.X, Is.EqualTo((int) control.X));
            Assert.That((int) test.Y, Is.EqualTo((int) control.Y));
        }
    }

    #endregion
}