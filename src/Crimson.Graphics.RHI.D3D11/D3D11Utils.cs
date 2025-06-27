using SharpGen.Runtime;

namespace Crimson.Graphics.RHI.D3D11;

internal static class D3D11Utils
{
    public static void Check(this Result result, string operation)
    {
        if (result.Failure)
            throw new Exception($"D3D11 operation '{operation}' failed: {result.ToString()}");
    }
}