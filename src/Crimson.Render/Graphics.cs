using System.Diagnostics;
using System.Numerics;
using Crimson.Math;
using Crimson.Render.Renderers;
using Crimson.Render.Renderers.Structs;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;

namespace Crimson.Render;

/// <summary>
/// The graphics subsystem, containing everything used to render.
/// </summary>
public sealed class Graphics : IDisposable
{
    private readonly IDXGISwapChain _swapchain;
    private ID3D11Texture2D _swapchainTexture;
    private ID3D11RenderTargetView _swapchainTarget;

    private Size<int> _swapchainSize;
    
    private readonly TextureBatcher _uiBatcher;
    private readonly DeferredRenderer _deferredRenderer;
    
    internal readonly ID3D11Device Device;
    internal readonly ID3D11DeviceContext Context;

    public Camera Camera;
    
    /// <summary>
    /// Create the graphics subsystem.
    /// </summary>
    /// <param name="appName">The application name.</param>
    /// <param name="info">The <see cref="SurfaceInfo"/> to use when creating the subsystem.</param>
    /// <param name="size">The size of the swapchain.</param>
    public Graphics(string appName, in SurfaceInfo info, Size<int> size)
    {
        _swapchainSize = size;

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
        _deferredRenderer = new DeferredRenderer(Device, size);

        Camera = new Camera()
        {
            ProjectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(float.DegreesToRadians(45),
                _swapchainSize.Width / (float)_swapchainSize.Height, 0.1f, 100f),
            ViewMatrix = Matrix4x4.CreateLookAt(new Vector3(0, 0, 3), Vector3.Zero, Vector3.UnitY)
        };
    }

    /// <summary>
    /// Destroy the graphics subsystem.
    /// </summary>
    public void Dispose()
    {
        _deferredRenderer.Dispose();
        _uiBatcher.Dispose();

        _swapchainTarget.Dispose();
        _swapchainTexture.Dispose();
        _swapchain.Dispose();
        Context.Dispose();
        Device.Dispose();
    }

    /// <summary>
    /// Create a <see cref="Renderable"/> that can be drawn.
    /// </summary>
    /// <param name="mesh">The mesh to use.</param>
    /// <returns>The created <see cref="Renderable"/>.</returns>
    public Renderable CreateRenderable(Mesh mesh)
    {
        return new Renderable(Device, mesh);
    }

    /// <summary>
    /// Create a <see cref="Texture"/>.
    /// </summary>
    /// <param name="size">The size, in pixels.</param>
    /// <param name="data">The data.</param>
    /// <param name="format">The <see cref="PixelFormat"/> of the texture.</param>
    public Texture CreateTexture(in Size<int> size, byte[] data, PixelFormat format = PixelFormat.RGBA8)
    {
        return new Texture(Device, Context, in size, data, format);
    }

    /// <summary>
    /// Create a <see cref="Texture"/> from the given bitmap.
    /// </summary>
    /// <param name="bitmap">The <see cref="Bitmap"/> to use.</param>
    public Texture CreateTexture(Bitmap bitmap)
        => CreateTexture(bitmap.Size, bitmap.Data, bitmap.Format);

    /// <summary>
    /// Create a <see cref="Texture"/> from the given path. 
    /// </summary>
    /// <param name="path">The path to load from.</param>
    public Texture CreateTexture(string path)
        => CreateTexture(new Bitmap(path));

    /// <summary>
    /// Draw a <see cref="Renderable"/> to the screen using the built-in renderers.
    /// </summary>
    /// <param name="renderable">The <see cref="Renderable"/> to draw.</param>
    /// <param name="worldMatrix">The world matrix.</param>
    public void DrawRenderable(Renderable renderable, Matrix4x4 worldMatrix)
    {
        _deferredRenderer.AddToQueue(renderable, worldMatrix);
    }

    /// <summary>
    /// Draw an image to the screen.
    /// </summary>
    /// <param name="texture">The texture to use as the image.</param>
    /// <param name="position">The position, in pixels.</param>
    public void DrawImage(Texture texture, in Vector2 position)
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
    public void Render()
    {
        Context.OMSetRenderTargets(_swapchainTarget);
        Context.ClearRenderTargetView(_swapchainTarget, new Color4(0.0f, 0.0f, 0.0f));

        Context.RSSetViewport(0, 0, _swapchainSize.Width, _swapchainSize.Height);
        
        _deferredRenderer.Render(Context, _swapchainTarget, Camera.Matrices);

        Matrix4x4 projection =
            Matrix4x4.CreateOrthographicOffCenter(0, _swapchainSize.Width, _swapchainSize.Height, 0, -1, 1);
        
        _uiBatcher.DispatchDrawQueue(Context, projection, Matrix4x4.Identity);
        
        _swapchain.Present(1);
    }
}