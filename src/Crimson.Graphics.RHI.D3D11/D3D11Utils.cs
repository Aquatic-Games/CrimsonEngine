global using DxgiFormat = Vortice.DXGI.Format;
using SharpGen.Runtime;

namespace Crimson.Graphics.RHI.D3D11;

internal static class D3D11Utils
{
    public static void Check(this Result result, string operation)
    {
        if (result.Failure)
            throw new Exception($"D3D11 operation '{operation}' failed: {result.ToString()}");
    }

    public static DxgiFormat ToD3D(this Format format)
    {
        return format switch
        {
            Format.R8B8B8A8_UNorm => DxgiFormat.R8G8B8A8_UNorm,
            Format.B8G8R8A8_UNorm => DxgiFormat.B8G8R8A8_UNorm,
            Format.R16_UInt => DxgiFormat.R16_UInt,
            Format.R32_UInt => DxgiFormat.R32_UInt,
            Format.R32_Float => DxgiFormat.R32_Float,
            Format.R32G32_Float => DxgiFormat.R32G32_Float,
            Format.R32G32B32_Float => DxgiFormat.R32G32B32_Float,
            Format.R32G32B32A32_Float => DxgiFormat.R32G32B32A32_Float,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }
    
    public static string ToD3D(this Semantic semantic)
    {
        return semantic switch
        {
            Semantic.TexCoord => "TEXCOORD",
            Semantic.Position => "POSITION",
            Semantic.Color => "COLOR",
            Semantic.Normal => "NORMAL",
            Semantic.Tangent => "TANGENT",
            Semantic.BiTangent => "BITANGENT",
            _ => throw new ArgumentOutOfRangeException(nameof(semantic), semantic, null)
        };
    }
}