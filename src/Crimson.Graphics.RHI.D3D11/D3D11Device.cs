using Crimson.Core;
using Crimson.Math;
using SDL3;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace Crimson.Graphics.RHI.D3D11;

public class D3D11Device : Device
{
    private readonly ID3D11Device _device;
    private readonly ID3D11DeviceContext _context;

    private readonly IDXGISwapChain _swapchain;
    private D3D11Texture _swapchainTexture;

    public override Backend Backend => Backend.D3D11;

    public override Format SwapchainFormat => Format.B8G8R8A8_UNorm;

    public D3D11Device(IntPtr sdlWindow, Size<uint> size, bool debug)
    {
        nint hwnd = SDL.GetPointerProperty(SDL.GetWindowProperties(sdlWindow), SDL.Props.WindowWin32HWNDPointer, 0);

        SwapChainDescription swapchainDesc = new()
        {
            OutputWindow = hwnd,
            Windowed = true,
            BufferCount = 2,
            BufferDescription = new ModeDescription(size.Width, size.Height, Vortice.DXGI.Format.B8G8R8A8_UNorm),
            BufferUsage = Usage.RenderTargetOutput,
            SwapEffect = SwapEffect.FlipDiscard,
            SampleDescription = new SampleDescription(1, 0),
            Flags = SwapChainFlags.AllowModeSwitch | SwapChainFlags.AllowTearing
        };
        
        DeviceCreationFlags flags = DeviceCreationFlags.BgraSupport;
        if (debug)
            flags |= DeviceCreationFlags.Debug;

        Logger.Trace("Creating device and swap chain.");
        Vortice.Direct3D11.D3D11
            .D3D11CreateDeviceAndSwapChain(null, DriverType.Hardware, flags, [FeatureLevel.Level_11_0], swapchainDesc,
                out _swapchain!, out _device!, out _, out _context!).Check("Create D3D11 device and swap chain");

        Logger.Trace("Creating swapchain render target.");
        _swapchainTexture = new D3D11Texture(_device, _swapchain.GetBuffer<ID3D11Texture2D>(0), size);
    }
    
    public override CommandList CreateCommandList()
    {
        return new D3D11CommandList(_device);
    }
    
    public override ShaderModule CreateShaderModule(byte[] compiled, string entryPoint)
    {
        return new D3D11ShaderModule(compiled);
    }
    
    public override Pipeline CreateGraphicsPipeline(in GraphicsPipelineInfo info)
    {
        return new D3D11Pipeline(_device, in info);
    }
    
    public override Buffer CreateBuffer(BufferUsage usage, uint sizeInBytes)
    {
        return new D3D11Buffer(_device, usage, sizeInBytes);
    }
    
    public override void ExecuteCommandList(CommandList cl)
    {
        D3D11CommandList d3dList = (D3D11CommandList) cl;
        _context.ExecuteCommandList(d3dList.CommandList, false);
    }
    
    public override nint MapBuffer(Buffer buffer)
    {
        D3D11Buffer d3dBuffer = (D3D11Buffer) buffer;
        MappedSubresource res = _context.Map(d3dBuffer.Buffer, MapMode.Write);
        return res.DataPointer;
    }
    
    public override void UnmapBuffer(Buffer buffer)
    {
        D3D11Buffer d3dBuffer = (D3D11Buffer) buffer;
        _context.Unmap(d3dBuffer.Buffer);
    }
    
    public override Texture GetNextSwapchainTexture()
    {
        return _swapchainTexture;
    }
    
    public override void Present()
    {
        // TODO: VSync
        _swapchain.Present(1).Check("Present");
    }
    
    public override void Resize(Size<uint> newSize)
    {
        throw new NotImplementedException();
    }
    
    public override void Dispose()
    {
        _swapchainTexture.Dispose();
        _swapchain.Dispose();
        _context.Dispose();
        _device.Dispose();
    }
}