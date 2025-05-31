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

    #endregion
}