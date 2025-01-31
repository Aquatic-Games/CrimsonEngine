namespace Euphoria.Math.Tests;

public class Vector2Tests
{
    [Test]
    public void TestConstruct()
    {
        Vector2T<int> vector = new Vector2T<int>(5, 10);
        
        Assert.Multiple(() =>
        {
            Assert.That(vector.X, Is.EqualTo(5));
            Assert.That(vector.Y, Is.EqualTo(10));
        });
    }
    
    [Test]
    public void TestConstructScalar()
    {
        Vector2T<int> vector = new Vector2T<int>(5);
        
        Assert.Multiple(() =>
        {
            Assert.That(vector.X, Is.EqualTo(5));
            Assert.That(vector.Y, Is.EqualTo(5));
        });
    }

    [Test]
    public void TestCasting()
    {
        Vector2T<float> fVector = new Vector2T<float>(1.5f, 2.9f);
        Vector2T<int> iVector = fVector.As<int>();
        
        Assert.Multiple(() =>
        {
            Assert.That(iVector.X, Is.EqualTo((int) fVector.X));
            Assert.That(iVector.Y, Is.EqualTo((int) fVector.Y));
        });
    }

    [Test]
    public void TestAddition()
    {
        Vector2T<int> a = new Vector2T<int>(1, 2);
        Vector2T<int> b = new Vector2T<int>(3, 4);

        Vector2T<int> c = a + b;
        
        Assert.Multiple(() =>
        {
            Assert.That(c.X, Is.EqualTo(1 + 3));
            Assert.That(c.Y, Is.EqualTo(2 + 4));
        });
    }

    [Test]
    public void TestSubtraction()
    {
        Vector2T<int> a = new Vector2T<int>(1, 2);
        Vector2T<int> b = new Vector2T<int>(3, 4);

        Vector2T<int> c = a - b;
        
        Assert.Multiple(() =>
        {
            Assert.That(c.X, Is.EqualTo(1 - 3));
            Assert.That(c.Y, Is.EqualTo(2 - 4));
        });
    }

    [Test]
    public void TestMultiplication()
    {
        Vector2T<int> a = new Vector2T<int>(1, 2);
        Vector2T<int> b = new Vector2T<int>(3, 4);

        Vector2T<int> c = a * b;
        
        Assert.Multiple(() =>
        {
            Assert.That(c.X, Is.EqualTo(1 * 3));
            Assert.That(c.Y, Is.EqualTo(2 * 4));
        });
    }

    [Test]
    public void TestMultiplicationScalar()
    {
        Vector2T<int> a = new Vector2T<int>(1, 2);
        const int b = 3;

        Vector2T<int> c = a * b;
        
        Assert.Multiple(() =>
        {
            Assert.That(c.X, Is.EqualTo(1 * 3));
            Assert.That(c.Y, Is.EqualTo(2 * 3));
        });
    }

    [Test]
    public void TestDivision()
    {
        Vector2T<float> a = new Vector2T<float>(1, 2);
        Vector2T<float> b = new Vector2T<float>(3, 4);

        Vector2T<float> c = a / b;
        
        Assert.Multiple(() =>
        {
            Assert.That(c.X, Is.EqualTo(1f / 3f));
            Assert.That(c.Y, Is.EqualTo(2f / 4f));
        });
    }

    [Test]
    public void TestDivisionScalar()
    {
        Vector2T<float> a = new Vector2T<float>(1, 2);
        const float b = 3;

        Vector2T<float> c = a / b;
        
        Assert.Multiple(() =>
        {
            Assert.That(c.X, Is.EqualTo(1f / 3f));
            Assert.That(c.Y, Is.EqualTo(2f / 3f));
        });
    }

    [Test]
    public void TestEquals()
    {
        Vector2T<int> a = new Vector2T<int>(1, 2);
        Vector2T<int> b = new Vector2T<int>(1, 2);

        Assert.That(a == b, Is.True);
        
        Vector2T<int> c = new Vector2T<int>(3, 4);
        
        Assert.That(a == c, Is.False);
    }

    [Test]
    public void TestNotEquals()
    {
        Vector2T<int> a = new Vector2T<int>(1, 2);
        Vector2T<int> b = new Vector2T<int>(1, 2);

        Assert.That(a != b, Is.False);
        
        Vector2T<int> c = new Vector2T<int>(3, 4);
        
        Assert.That(a != c, Is.True);
    }

    [Test]
    public void TestToString()
    {
        Vector2T<int> vector = new Vector2T<int>(1, 2);
        string str = vector.ToString();
        
        Assert.That(str, Is.EqualTo("X: 1, Y: 2"));
    }
}