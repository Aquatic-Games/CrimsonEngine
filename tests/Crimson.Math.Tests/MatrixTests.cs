using System.Numerics;

namespace Crimson.Math.Tests;

public class MatrixTests
{
    [Test]
    public void TestConstruct()
    {
        Matrix<int> matrix = new Matrix<int>(new Vector4T<int>(1, 2, 3, 4), new Vector4T<int>(5, 6, 7, 8),
            new Vector4T<int>(9, 10, 11, 12), new Vector4T<int>(13, 14, 15, 16));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(matrix.Row0.X, Is.EqualTo(1));
            Assert.That(matrix.Row0.Y, Is.EqualTo(2));
            Assert.That(matrix.Row0.Z, Is.EqualTo(3));
            Assert.That(matrix.Row0.W, Is.EqualTo(4));
            
            Assert.That(matrix.Row1.X, Is.EqualTo(5));
            Assert.That(matrix.Row1.Y, Is.EqualTo(6));
            Assert.That(matrix.Row1.Z, Is.EqualTo(7));
            Assert.That(matrix.Row1.W, Is.EqualTo(8));
            
            Assert.That(matrix.Row2.X, Is.EqualTo(9));
            Assert.That(matrix.Row2.Y, Is.EqualTo(10));
            Assert.That(matrix.Row2.Z, Is.EqualTo(11));
            Assert.That(matrix.Row2.W, Is.EqualTo(12));
            
            Assert.That(matrix.Row3.X, Is.EqualTo(13));
            Assert.That(matrix.Row3.Y, Is.EqualTo(14));
            Assert.That(matrix.Row3.Z, Is.EqualTo(15));
            Assert.That(matrix.Row3.W, Is.EqualTo(16));
        }
    }

    [Test]
    public void TestConstructIndividual()
    {
        Matrix<int> matrix = new Matrix<int>(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(matrix.Row0.X, Is.EqualTo(1));
            Assert.That(matrix.Row0.Y, Is.EqualTo(2));
            Assert.That(matrix.Row0.Z, Is.EqualTo(3));
            Assert.That(matrix.Row0.W, Is.EqualTo(4));
            
            Assert.That(matrix.Row1.X, Is.EqualTo(5));
            Assert.That(matrix.Row1.Y, Is.EqualTo(6));
            Assert.That(matrix.Row1.Z, Is.EqualTo(7));
            Assert.That(matrix.Row1.W, Is.EqualTo(8));
            
            Assert.That(matrix.Row2.X, Is.EqualTo(9));
            Assert.That(matrix.Row2.Y, Is.EqualTo(10));
            Assert.That(matrix.Row2.Z, Is.EqualTo(11));
            Assert.That(matrix.Row2.W, Is.EqualTo(12));
            
            Assert.That(matrix.Row3.X, Is.EqualTo(13));
            Assert.That(matrix.Row3.Y, Is.EqualTo(14));
            Assert.That(matrix.Row3.Z, Is.EqualTo(15));
            Assert.That(matrix.Row3.W, Is.EqualTo(16));
        }
    }

    [Test]
    public void TestIdentity()
    {
        Matrix<int> matrix = Matrix<int>.Identity;
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(matrix.Row0.X, Is.EqualTo(1));
            Assert.That(matrix.Row0.Y, Is.EqualTo(0));
            Assert.That(matrix.Row0.Z, Is.EqualTo(0));
            Assert.That(matrix.Row0.W, Is.EqualTo(0));
            
            Assert.That(matrix.Row1.X, Is.EqualTo(0));
            Assert.That(matrix.Row1.Y, Is.EqualTo(1));
            Assert.That(matrix.Row1.Z, Is.EqualTo(0));
            Assert.That(matrix.Row1.W, Is.EqualTo(0));
            
            Assert.That(matrix.Row2.X, Is.EqualTo(0));
            Assert.That(matrix.Row2.Y, Is.EqualTo(0));
            Assert.That(matrix.Row2.Z, Is.EqualTo(1));
            Assert.That(matrix.Row2.W, Is.EqualTo(0));
            
            Assert.That(matrix.Row3.X, Is.EqualTo(0));
            Assert.That(matrix.Row3.Y, Is.EqualTo(0));
            Assert.That(matrix.Row3.Z, Is.EqualTo(0));
            Assert.That(matrix.Row3.W, Is.EqualTo(1));
        }
    }

    [Test]
    public void TestColumns()
    {
        Matrix<int> matrix = new Matrix<int>(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(matrix.Column0.X, Is.EqualTo(1));
            Assert.That(matrix.Column0.Y, Is.EqualTo(5));
            Assert.That(matrix.Column0.Z, Is.EqualTo(9));
            Assert.That(matrix.Column0.W, Is.EqualTo(13));
            
            Assert.That(matrix.Column1.X, Is.EqualTo(2));
            Assert.That(matrix.Column1.Y, Is.EqualTo(6));
            Assert.That(matrix.Column1.Z, Is.EqualTo(10));
            Assert.That(matrix.Column1.W, Is.EqualTo(14));
            
            Assert.That(matrix.Column2.X, Is.EqualTo(3));
            Assert.That(matrix.Column2.Y, Is.EqualTo(7));
            Assert.That(matrix.Column2.Z, Is.EqualTo(11));
            Assert.That(matrix.Column2.W, Is.EqualTo(15));
            
            Assert.That(matrix.Column3.X, Is.EqualTo(4));
            Assert.That(matrix.Column3.Y, Is.EqualTo(8));
            Assert.That(matrix.Column3.Z, Is.EqualTo(12));
            Assert.That(matrix.Column3.W, Is.EqualTo(16));
        }
    }

    [Test]
    public void TestIndexer()
    {
        Matrix<int> matrix = new Matrix<int>(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(matrix[0][0], Is.EqualTo(1));
            Assert.That(matrix[0][1], Is.EqualTo(2));
            Assert.That(matrix[0][2], Is.EqualTo(3));
            Assert.That(matrix[0][3], Is.EqualTo(4));
            
            Assert.That(matrix[1][0], Is.EqualTo(5));
            Assert.That(matrix[1][1], Is.EqualTo(6));
            Assert.That(matrix[1][2], Is.EqualTo(7));
            Assert.That(matrix[1][3], Is.EqualTo(8));
            
            Assert.That(matrix[2][0], Is.EqualTo(9));
            Assert.That(matrix[2][1], Is.EqualTo(10));
            Assert.That(matrix[2][2], Is.EqualTo(11));
            Assert.That(matrix[2][3], Is.EqualTo(12));
            
            Assert.That(matrix[3][0], Is.EqualTo(13));
            Assert.That(matrix[3][1], Is.EqualTo(14));
            Assert.That(matrix[3][2], Is.EqualTo(15));
            Assert.That(matrix[3][3], Is.EqualTo(16));
        }
    }

    [Test]
    public void TestAddition()
    {
        Matrix<int> a = new Matrix<int>(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
        Matrix<int> b = new Matrix<int>(17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32);

        Matrix<int> c = a + b;
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.Row0.X, Is.EqualTo(1 + 17));
            Assert.That(c.Row0.Y, Is.EqualTo(2 + 18));
            Assert.That(c.Row0.Z, Is.EqualTo(3 + 19));
            Assert.That(c.Row0.W, Is.EqualTo(4 + 20));
            
            Assert.That(c.Row1.X, Is.EqualTo(5 + 21));
            Assert.That(c.Row1.Y, Is.EqualTo(6 + 22));
            Assert.That(c.Row1.Z, Is.EqualTo(7 + 23));
            Assert.That(c.Row1.W, Is.EqualTo(8 + 24));
            
            Assert.That(c.Row2.X, Is.EqualTo(9 + 25));
            Assert.That(c.Row2.Y, Is.EqualTo(10 + 26));
            Assert.That(c.Row2.Z, Is.EqualTo(11 + 27));
            Assert.That(c.Row2.W, Is.EqualTo(12 + 28));
            
            Assert.That(c.Row3.X, Is.EqualTo(13 + 29));
            Assert.That(c.Row3.Y, Is.EqualTo(14 + 30));
            Assert.That(c.Row3.Z, Is.EqualTo(15 + 31));
            Assert.That(c.Row3.W, Is.EqualTo(16 + 32));
        }
    }
    
    [Test]
    public void TestSubtraction()
    {
        Matrix<int> a = new Matrix<int>(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
        Matrix<int> b = new Matrix<int>(17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32);

        Matrix<int> c = a - b;
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(c.Row0.X, Is.EqualTo(1 - 17));
            Assert.That(c.Row0.Y, Is.EqualTo(2 - 18));
            Assert.That(c.Row0.Z, Is.EqualTo(3 - 19));
            Assert.That(c.Row0.W, Is.EqualTo(4 - 20));
            
            Assert.That(c.Row1.X, Is.EqualTo(5 - 21));
            Assert.That(c.Row1.Y, Is.EqualTo(6 - 22));
            Assert.That(c.Row1.Z, Is.EqualTo(7 - 23));
            Assert.That(c.Row1.W, Is.EqualTo(8 - 24));
            
            Assert.That(c.Row2.X, Is.EqualTo(9 -25));
            Assert.That(c.Row2.Y, Is.EqualTo(10 - 26));
            Assert.That(c.Row2.Z, Is.EqualTo(11 - 27));
            Assert.That(c.Row2.W, Is.EqualTo(12 - 28));
            
            Assert.That(c.Row3.X, Is.EqualTo(13 - 29));
            Assert.That(c.Row3.Y, Is.EqualTo(14 - 30));
            Assert.That(c.Row3.Z, Is.EqualTo(15 - 31));
            Assert.That(c.Row3.W, Is.EqualTo(16 - 32));
        }
    }

    [Test]
    public void TestMultiplication()
    {
        Matrix4x4 controlA = new Matrix4x4(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
        Matrix4x4 controlB = new Matrix4x4(17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32);
        Matrix4x4 controlC = controlA * controlB;
        
        Matrix<int> testA = new Matrix<int>(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
        Matrix<int> testB = new Matrix<int>(17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32);
        Matrix<int> testC = testA * testB;
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(testC.Row0.X, Is.EqualTo(controlC.M11));
            Assert.That(testC.Row0.Y, Is.EqualTo(controlC.M12));
            Assert.That(testC.Row0.Z, Is.EqualTo(controlC.M13));
            Assert.That(testC.Row0.W, Is.EqualTo(controlC.M14));
            
            Assert.That(testC.Row1.X, Is.EqualTo(controlC.M21));
            Assert.That(testC.Row1.Y, Is.EqualTo(controlC.M22));
            Assert.That(testC.Row1.Z, Is.EqualTo(controlC.M23));
            Assert.That(testC.Row1.W, Is.EqualTo(controlC.M24));
            
            Assert.That(testC.Row2.X, Is.EqualTo(controlC.M31));
            Assert.That(testC.Row2.Y, Is.EqualTo(controlC.M32));
            Assert.That(testC.Row2.Z, Is.EqualTo(controlC.M33));
            Assert.That(testC.Row2.W, Is.EqualTo(controlC.M34));
            
            Assert.That(testC.Row3.X, Is.EqualTo(controlC.M41));
            Assert.That(testC.Row3.Y, Is.EqualTo(controlC.M42));
            Assert.That(testC.Row3.Z, Is.EqualTo(controlC.M43));
            Assert.That(testC.Row3.W, Is.EqualTo(controlC.M44));
        }
    }
}