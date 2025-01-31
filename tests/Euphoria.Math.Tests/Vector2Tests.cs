namespace Euphoria.Math.Tests;

public class Vector2Tests
{
    [Test]
    public void Construct()
    {
        Vector2T<int> vector = new Vector2T<int>(5, 10);
        
        Assert.Multiple(() =>
        {
            Assert.That(vector.X, Is.EqualTo(5));
            Assert.That(vector.Y, Is.EqualTo(10));
        });
    }
    
    [Test]
    public void ConstructScalar()
    {
        Vector2T<int> vector = new Vector2T<int>(5);
        
        Assert.Multiple(() =>
        {
            Assert.That(vector.X, Is.EqualTo(5));
            Assert.That(vector.Y, Is.EqualTo(5));
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
}