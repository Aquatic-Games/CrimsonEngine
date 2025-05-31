using System.Numerics;

namespace Crimson.Math.Tests;

public class Vector4Tests
{
    #region Struct
    
    [Test]
    public void TestConstruct()
    {
        Vector4T<int> vector = new Vector4T<int>(1, 2, 3, 4);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vector.X, Is.EqualTo(1));
            Assert.That(vector.Y, Is.EqualTo(2));
            Assert.That(vector.Z, Is.EqualTo(3));
            Assert.That(vector.W, Is.EqualTo(4));
        }
    }

    [Test]
    public void TestConstructScalar()
    {
        Vector4T<int> vector = new Vector4T<int>(2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vector.X, Is.EqualTo(2));
            Assert.That(vector.Y, Is.EqualTo(2));
            Assert.That(vector.Z, Is.EqualTo(2));
            Assert.That(vector.W, Is.EqualTo(2));
        }
    }

    [Test]
    public void TestZero()
    {
        Vector4T<int> vector = Vector4T<int>.Zero;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vector.X, Is.EqualTo(0));
            Assert.That(vector.Y, Is.EqualTo(0));
            Assert.That(vector.Z, Is.EqualTo(0));
            Assert.That(vector.W, Is.EqualTo(0));
        }
    }

    [Test]
    public void TestOne()
    {
        Vector4T<int> vector = Vector4T<int>.One;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vector.X, Is.EqualTo(1));
            Assert.That(vector.Y, Is.EqualTo(1));
            Assert.That(vector.Z, Is.EqualTo(1));
            Assert.That(vector.W, Is.EqualTo(1));
        }
    }

    [Test]
    public void TestUnitX()
    {
        Vector4T<int> vector = Vector4T<int>.UnitX;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vector.X, Is.EqualTo(1));
            Assert.That(vector.Y, Is.EqualTo(0));
            Assert.That(vector.Z, Is.EqualTo(0));
            Assert.That(vector.W, Is.EqualTo(0));
        }
    }
    
    [Test]
    public void TestUnitY()
    {
        Vector4T<int> vector = Vector4T<int>.UnitY;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vector.X, Is.EqualTo(0));
            Assert.That(vector.Y, Is.EqualTo(1));
            Assert.That(vector.Z, Is.EqualTo(0));
            Assert.That(vector.W, Is.EqualTo(0));
        }
    }
    
    [Test]
    public void TestUnitZ()
    {
        Vector4T<int> vector = Vector4T<int>.UnitZ;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vector.X, Is.EqualTo(0));
            Assert.That(vector.Y, Is.EqualTo(0));
            Assert.That(vector.Z, Is.EqualTo(1));
            Assert.That(vector.W, Is.EqualTo(0));
        }
    }
    
    [Test]
    public void TestUnitW()
    {
        Vector4T<int> vector = Vector4T<int>.UnitW;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vector.X, Is.EqualTo(0));
            Assert.That(vector.Y, Is.EqualTo(0));
            Assert.That(vector.Z, Is.EqualTo(0));
            Assert.That(vector.W, Is.EqualTo(1));
        }
    }

    [Test]
    public void TestIndexer()
    {
        Vector4T<int> vector = new Vector4T<int>(1, 2, 3, 4);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vector[0], Is.EqualTo(1));
            Assert.That(vector[1], Is.EqualTo(2));
            Assert.That(vector[2], Is.EqualTo(3));
            Assert.That(vector[3], Is.EqualTo(4));
        }
    }

    [Test]
    public void TestAs()
    {
        Vector4T<float> floatVector = new Vector4T<float>(3.7f, -2.3f, 1, -4.9f);
        Vector4T<int> intVector = floatVector.As<int>();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(intVector.X, Is.EqualTo(3));
            Assert.That(intVector.Y, Is.EqualTo(-2));
            Assert.That(intVector.Z, Is.EqualTo(1));
            Assert.That(intVector.W, Is.EqualTo(-4));
        }
    }

    [Test]
    public void TestVectorVectorAddition()
    {
        Vector4T<int> a = new Vector4T<int>(1, 2, 3, 4);
        Vector4T<int> b = new Vector4T<int>(5, 6, 7, 8);

        Vector4T<int> c = a + b;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.X, Is.EqualTo(1 + 5));
            Assert.That(c.Y, Is.EqualTo(2 + 6));
            Assert.That(c.Z, Is.EqualTo(3 + 7));
            Assert.That(c.W, Is.EqualTo(4 + 8));
        }
    }
    
    [Test]
    public void TestVectorVectorSubtraction()
    {
        Vector4T<int> a = new Vector4T<int>(1, 2, 3, 4);
        Vector4T<int> b = new Vector4T<int>(5, 6, 7, 8);

        Vector4T<int> c = a - b;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.X, Is.EqualTo(1 - 5));
            Assert.That(c.Y, Is.EqualTo(2 - 6));
            Assert.That(c.Z, Is.EqualTo(3 - 7));
            Assert.That(c.W, Is.EqualTo(4 - 8));
        }
    }
    
    [Test]
    public void TestVectorVectorMultiplication()
    {
        Vector4T<int> a = new Vector4T<int>(1, 2, 3, 4);
        Vector4T<int> b = new Vector4T<int>(5, 6, 7, 8);

        Vector4T<int> c = a * b;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.X, Is.EqualTo(1 * 5));
            Assert.That(c.Y, Is.EqualTo(2 * 6));
            Assert.That(c.Z, Is.EqualTo(3 * 7));
            Assert.That(c.W, Is.EqualTo(4 * 8));
        }
    }
    
    [Test]
    public void TestVectorScalarMultiplication()
    {
        Vector4T<int> a = new Vector4T<int>(1, 2, 3, 4);
        const int b = 4;

        Vector4T<int> c = a * b;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.X, Is.EqualTo(1 * 4));
            Assert.That(c.Y, Is.EqualTo(2 * 4));
            Assert.That(c.Z, Is.EqualTo(3 * 4));
            Assert.That(c.W, Is.EqualTo(4 * 4));
        }
    }
    
    [Test]
    public void TestVectorVectorDivision()
    {
        Vector4T<float> a = new Vector4T<float>(1, 2, 3, 4);
        Vector4T<float> b = new Vector4T<float>(5, 6, 7, 8);

        Vector4T<float> c = a / b;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.X, Is.EqualTo(1.0f / 5.0f));
            Assert.That(c.Y, Is.EqualTo(2.0f / 6.0f));
            Assert.That(c.Z, Is.EqualTo(3.0f / 7.0f));
            Assert.That(c.W, Is.EqualTo(4.0f / 8.0f));
        }
    }
    
    [Test]
    public void TestVectorScalarDivision()
    {
        Vector4T<float> a = new Vector4T<float>(1, 2, 3, 4);
        const float b = 4;

        Vector4T<float> c = a / b;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.X, Is.EqualTo(1.0f / 4.0f));
            Assert.That(c.Y, Is.EqualTo(2.0f / 4.0f));
            Assert.That(c.Z, Is.EqualTo(3.0f / 4.0f));
            Assert.That(c.W, Is.EqualTo(4.0f / 4.0f));
        }
    }

    [Test]
    public void TestEquals()
    {
        Vector4T<int> a = new Vector4T<int>(1, 2, 3, 4);
        Vector4T<int> b = new Vector4T<int>(4, 3, 2, 1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(a == a, Is.True);
            Assert.That(a == b, Is.False);
        }
    }

    [Test]
    public void TestNotEquals()
    {
        Vector4T<int> a = new Vector4T<int>(1, 2, 3, 4);
        Vector4T<int> b = new Vector4T<int>(4, 3, 2, 1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(a != a, Is.False);
            Assert.That(a != b, Is.True);
        }
    }

    [Test]
    public void TestToString()
    {
        Vector4T<int> vector = new Vector4T<int>(1, 2, 3, 4);
        Assert.That(vector.ToString(), Is.EqualTo("X: 1, Y: 2, Z: 3, W: 4"));
    }
    
    #endregion

    #region Operations

    [Test]
    public void TestDot()
    {
        float control = Vector4.Dot(new Vector4(1, 2, 3, 4), new Vector4(4, 3, 2, 1));
        float test = Vector4T.Dot(new Vector4T<float>(1, 2, 3, 4), new Vector4T<float>(4, 3, 2, 1));
        
        Assert.That(test, Is.EqualTo(control));
    }

    [Test]
    public void TestMagnitudeSquared()
    {
        float control = new Vector4(1, 2, 3, 4).LengthSquared();
        float test = Vector4T.MagnitudeSquared(new Vector4T<float>(1, 2, 3, 4));
        
        Assert.That(test, Is.EqualTo(control));
    }

    [Test]
    public void TestMagnitude()
    {
        float control = new Vector4(1, 2, 3, 4).Length();
        float test = Vector4T.Magnitude(new Vector4T<float>(1, 2, 3, 4));
        
        Assert.That(test, Is.EqualTo(control));
    }

    #endregion
}