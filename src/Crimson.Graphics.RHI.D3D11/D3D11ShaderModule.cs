namespace Crimson.Graphics.RHI.D3D11;

internal sealed class D3D11ShaderModule : ShaderModule
{
    public readonly byte[] Data;
    public readonly string EntryPoint;

    public D3D11ShaderModule(byte[] data, string entryPoint)
    {
        Data = data;
        EntryPoint = entryPoint;
    }
    
    public override void Dispose() { }
}