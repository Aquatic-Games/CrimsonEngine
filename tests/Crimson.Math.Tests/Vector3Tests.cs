namespace Crimson.Math.Tests;

public class Vector3Tests
{
    #region Struct
    
    [Test]
    public void TestConstruct()
    {
        Vector3T<int> vector = new Vector3T<int>(1, 2, 3);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vector.X, Is.EqualTo(1));
            Assert.That(vector.Y, Is.EqualTo(2));
            Assert.That(vector.Z, Is.EqualTo(3));
        }
    }

    [Test]
    public void TestConstructScalar()
    {
        Vector3T<int> vector = new Vector3T<int>(2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vector.X, Is.EqualTo(2));
            Assert.That(vector.Y, Is.EqualTo(2));
            Assert.That(vector.Z, Is.EqualTo(2));
        }
    }

    [Test]
    public void TestZero()
    {
        Vector3T<int> vector = Vector3T<int>.Zero;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vector.X, Is.EqualTo(0));
            Assert.That(vector.Y, Is.EqualTo(0));
            Assert.That(vector.Z, Is.EqualTo(0));
        }
    }

    [Test]
    public void TestOne()
    {
        Vector3T<int> vector = Vector3T<int>.One;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vector.X, Is.EqualTo(1));
            Assert.That(vector.Y, Is.EqualTo(1));
            Assert.That(vector.Z, Is.EqualTo(1));
        }
    }

    [Test]
    public void TestUnitX()
    {
        Vector3T<int> vector = Vector3T<int>.UnitX;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vector.X, Is.EqualTo(1));
            Assert.That(vector.Y, Is.EqualTo(0));
            Assert.That(vector.Z, Is.EqualTo(0));
        }
    }
    
    [Test]
    public void TestUnitY()
    {
        Vector3T<int> vector = Vector3T<int>.UnitY;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vector.X, Is.EqualTo(0));
            Assert.That(vector.Y, Is.EqualTo(1));
            Assert.That(vector.Z, Is.EqualTo(0));
        }
    }
    
    [Test]
    public void TestUnitZ()
    {
        Vector3T<int> vector = Vector3T<int>.UnitZ;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vector.X, Is.EqualTo(0));
            Assert.That(vector.Y, Is.EqualTo(0));
            Assert.That(vector.Z, Is.EqualTo(1));
        }
    }

    [Test]
    public void TestIndexer()
    {
        Vector3T<int> vector = new Vector3T<int>(1, 2, 3);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vector[0], Is.EqualTo(1));
            Assert.That(vector[1], Is.EqualTo(2));
            Assert.That(vector[2], Is.EqualTo(3));
        }
    }

    [Test]
    public void TestAs()
    {
        Vector3T<float> floatVector = new Vector3T<float>(3.7f, -2.3f, 1);
        Vector3T<int> intVector = floatVector.As<int>();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(intVector.X, Is.EqualTo(3));
            Assert.That(intVector.Y, Is.EqualTo(-2));
            Assert.That(intVector.Z, Is.EqualTo(1));
        }
    }

    [Test]
    public void TestVectorVectorAddition()
    {
        Vector3T<int> a = new Vector3T<int>(1, 2, 3);
        Vector3T<int> b = new Vector3T<int>(5, 6, 7);

        Vector3T<int> c = a + b;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.X, Is.EqualTo(1 + 5));
            Assert.That(c.Y, Is.EqualTo(2 + 6));
            Assert.That(c.Z, Is.EqualTo(3 + 7));
        }
    }
    
    [Test]
    public void TestVectorVectorSubtraction()
    {
        Vector3T<int> a = new Vector3T<int>(1, 2, 3);
        Vector3T<int> b = new Vector3T<int>(5, 6, 7);

        Vector3T<int> c = a - b;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.X, Is.EqualTo(1 - 5));
            Assert.That(c.Y, Is.EqualTo(2 - 6));
            Assert.That(c.Z, Is.EqualTo(3 - 7));
        }
    }
    
    [Test]
    public void TestVectorVectorMultiplication()
    {
        Vector3T<int> a = new Vector3T<int>(1, 2, 3);
        Vector3T<int> b = new Vector3T<int>(5, 6, 7);

        Vector3T<int> c = a * b;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.X, Is.EqualTo(1 * 5));
            Assert.That(c.Y, Is.EqualTo(2 * 6));
            Assert.That(c.Z, Is.EqualTo(3 * 7));
        }
    }
    
    [Test]
    public void TestVectorScalarMultiplication()
    {
        Vector3T<int> a = new Vector3T<int>(1, 2, 3);
        const int b = 4;

        Vector3T<int> c = a * b;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.X, Is.EqualTo(1 * 4));
            Assert.That(c.Y, Is.EqualTo(2 * 4));
            Assert.That(c.Z, Is.EqualTo(3 * 4));
        }
    }
    
    [Test]
    public void TestVectorVectorDivision()
    {
        Vector3T<float> a = new Vector3T<float>(1, 2, 3);
        Vector3T<float> b = new Vector3T<float>(5, 6, 7);

        Vector3T<float> c = a / b;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.X, Is.EqualTo(1.0f / 5.0f));
            Assert.That(c.Y, Is.EqualTo(2.0f / 6.0f));
            Assert.That(c.Z, Is.EqualTo(3.0f / 7.0f));
        }
    }
    
    [Test]
    public void TestVectorScalarDivision()
    {
        Vector3T<float> a = new Vector3T<float>(1, 2, 3);
        const float b = 4;

        Vector3T<float> c = a / b;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.X, Is.EqualTo(1.0f / 4.0f));
            Assert.That(c.Y, Is.EqualTo(2.0f / 4.0f));
            Assert.That(c.Z, Is.EqualTo(3.0f / 4.0f));
        }
    }

    [Test]
    public void TestEquals()
    {
        Vector3T<int> a = new Vector3T<int>(1, 2, 3);
        Vector3T<int> b = new Vector3T<int>(4, 3, 2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(a == a, Is.True);
            Assert.That(a == b, Is.False);
        }
    }

    [Test]
    public void TestNotEquals()
    {
        Vector3T<int> a = new Vector3T<int>(1, 2, 3);
        Vector3T<int> b = new Vector3T<int>(4, 3, 2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(a != a, Is.False);
            Assert.That(a != b, Is.True);
        }
    }

    [Test]
    public void TestToString()
    {
        Vector3T<int> vector = new Vector3T<int>(1, 2, 3);
        Assert.That(vector.ToString(), Is.EqualTo("X: 1, Y: 2, Z: 3"));
    }
    
    #endregion
}