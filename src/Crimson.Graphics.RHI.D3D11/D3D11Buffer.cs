using Crimson.Core;
using Vortice.Direct3D11;

namespace Crimson.Graphics.RHI.D3D11;

internal sealed class D3D11Buffer : Buffer
{
    public readonly ID3D11Buffer Buffer;
    public readonly uint BufferSize;

    public D3D11Buffer(ID3D11Device device, BufferUsage usage, uint sizeInBytes)
    {
        BufferSize = sizeInBytes;
        
        BindFlags flags = BindFlags.None;
        CpuAccessFlags cpuAccess = CpuAccessFlags.None;
        ResourceUsage resUsage = ResourceUsage.Default;

        if ((usage & BufferUsage.VertexBuffer) != 0)
            flags |= BindFlags.VertexBuffer;
        if ((usage & BufferUsage.IndexBuffer) != 0)
            flags |= BindFlags.IndexBuffer;
        if ((usage & BufferUsage.ConstantBuffer) != 0)
            flags |= BindFlags.ConstantBuffer;

        if ((usage & BufferUsage.TransferSrc) != 0)
        {
            cpuAccess = CpuAccessFlags.Write;
            resUsage = ResourceUsage.Staging;
        }

        BufferDescription desc = new()
        {
            BindFlags = flags,
            ByteWidth = sizeInBytes,
            CPUAccessFlags = cpuAccess,
            Usage = resUsage
        };

        Logger.Trace("Creating buffer.");
        Buffer = device.CreateBuffer(in desc);
    }
    
    public override void Dispose()
    {
        Buffer.Dispose();
    }
}