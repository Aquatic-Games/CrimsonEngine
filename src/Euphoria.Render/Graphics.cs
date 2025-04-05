using System.Diagnostics;
using System.Numerics;
using Euphoria.Math;
using Euphoria.Render.Renderers;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;

namespace Euphoria.Render;

/// <summary>
/// The graphics subsystem, containing everything used to render.
/// </summary>
public static class Graphics
{
    private static IDXGISwapChain _swapchain;
    private static ID3D11Texture2D _swapchainTexture;
    private static ID3D11RenderTargetView _swapchainTarget;

    private static TextureBatcher _uiBatcher;
    
    internal static ID3D11Device Device = null!;
    internal static ID3D11DeviceContext Context = null!;
    
    /// <summary>
    /// Create the graphics subsystem.
    /// </summary>
    /// <param name="appName">The application name.</param>
    /// <param name="info">The <see cref="SurfaceInfo"/> to use when creating the subsystem.</param>
    /// <param name="size">The size of the swapchain.</param>
    public static void Create(string appName, in SurfaceInfo info, Size<int> size)
    {
        Debug.Assert(Device == null);

        SwapChainDescription swapchainDesc = new()
        {
            OutputWindow = info.NativeHandle,
            Windowed = true,
            BufferDescription = new ModeDescription((uint) size.Width, (uint) size.Height, Format.B8G8R8A8_UNorm),
            BufferUsage = Usage.RenderTargetOutput,
            BufferCount = 2,
            SampleDescription = new SampleDescription(1, 0),
            SwapEffect = SwapEffect.FlipDiscard,
            Flags = SwapChainFlags.None
        };

        DeviceCreationFlags flags = DeviceCreationFlags.Debug | DeviceCreationFlags.BgraSupport;
        FeatureLevel[] levels = [FeatureLevel.Level_11_1];

        D3D11.D3D11CreateDeviceAndSwapChain(null, DriverType.Hardware, flags, levels, swapchainDesc, out _swapchain!,
            out Device!, out _, out Context!).CheckError();

        _swapchainTexture = _swapchain.GetBuffer<ID3D11Texture2D>(0);
        _swapchainTarget = Device.CreateRenderTargetView(_swapchainTexture);

        _uiBatcher = new TextureBatcher(Device);
    }

    /// <summary>
    /// Destroy the graphics subsystem.
    /// </summary>
    public static void Destroy()
    {
        Debug.Assert(Device != null);
        
        _uiBatcher.Dispose();

        _swapchainTarget.Dispose();
        _swapchainTexture.Dispose();
        _swapchain.Dispose();
        Context.Dispose();
        Device.Dispose();
    }

    /// <summary>
    /// Draw an image to the screen.
    /// </summary>
    /// <param name="texture">The texture to use as the image.</param>
    /// <param name="position">The position, in pixels.</param>
    public static void DrawImage(Texture texture, in Vector2 position)
    {
        Size<int> size = texture.Size;
        
        Vector2 topLeft = position;
        Vector2 topRight = position + new Vector2(size.Width, 0);
        Vector2 bottomLeft = position + new Vector2(0, size.Height);
        Vector2 bottomRight = position + new Vector2(size.Width, size.Height);

        _uiBatcher.AddToDrawQueue(new TextureBatcher.Draw(texture, topLeft, topRight, bottomLeft, bottomRight));
    }

    /// <summary>
    /// Render and present to the surface.
    /// </summary>
    public static void Render()
    {
        Context.OMSetRenderTargets(_swapchainTarget);
        Context.ClearRenderTargetView(_swapchainTarget, new Color4(1.0f, 0.5f, 0.25f));

        Context.RSSetViewport(0, 0, 1280, 720);
        
        _uiBatcher.DispatchDrawQueue(Context, Matrix4x4.CreateOrthographicOffCenter(0, 1280, 720, 0, -1, 1),
            Matrix4x4.Identity);
        
        _swapchain.Present(1);
    }
}