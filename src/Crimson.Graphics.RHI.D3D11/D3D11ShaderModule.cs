namespace Crimson.Graphics.RHI.D3D11;

internal sealed class D3D11ShaderModule : ShaderModule
{
    public readonly byte[] Data;

    public D3D11ShaderModule(byte[] data)
    {
        Data = data;
    }
    
    public override void Dispose() { }
}