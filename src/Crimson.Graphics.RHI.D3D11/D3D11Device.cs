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

    public override Backend Backend => Backend.D3D11;
    public override Format SwapchainFormat { get; }

    public D3D11Device(IntPtr sdlWindow, Size<uint> size, bool debug)
    {
        nint hwnd = SDL.GetPointerProperty(SDL.GetWindowProperties(sdlWindow), SDL.Props.WindowWin32HWNDPointer, 0);

        SwapChainDescription swapchainDesc = new()
        {
            OutputWindow = hwnd,
            Windowed = true,
            BufferCount = 2,
            BufferDescription = new ModeDescription(size.Width, size.Height, Vortice.DXGI.Format.B8G8R8A8_UNorm),
            BufferUsage = Usage.Backbuffer,
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
    }
    
    public override CommandList CreateCommandList()
    {
        throw new NotImplementedException();
    }
    
    public override ShaderModule CreateShaderModule(ShaderStage stage, byte[] compiled, string entryPoint)
    {
        throw new NotImplementedException();
    }
    
    public override Pipeline CreateGraphicsPipeline(in GraphicsPipelineInfo info)
    {
        throw new NotImplementedException();
    }
    
    public override Buffer CreateBuffer(BufferUsage usage, uint sizeInBytes)
    {
        throw new NotImplementedException();
    }
    
    public override void ExecuteCommandList(CommandList cl)
    {
        throw new NotImplementedException();
    }
    
    public override IntPtr MapBuffer(Buffer buffer)
    {
        throw new NotImplementedException();
    }
    
    public override void UnmapBuffer(Buffer buffer)
    {
        throw new NotImplementedException();
    }
    
    public override Texture GetNextSwapchainTexture()
    {
        throw new NotImplementedException();
    }
    
    public override void Present()
    {
        throw new NotImplementedException();
    }
    
    public override void Resize(Size<uint> newSize)
    {
        throw new NotImplementedException();
    }
    
    public override void Dispose()
    {
        _swapchain.Dispose();
        _context.Dispose();
        _device.Dispose();
    }
}