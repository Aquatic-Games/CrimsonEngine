using System.Diagnostics;
using System.Numerics;
using Crimson.Core;
using Crimson.Graphics.Renderers;
using Crimson.Graphics.Utils;
using Crimson.Math;
using Hexa.NET.ImGui;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;
using Color = Crimson.Math.Color;

namespace Crimson.Graphics;

/// <summary>
/// The graphics subsystem, containing everything used to render.
/// </summary>
public sealed class Renderer : IDisposable
{
    private readonly IDXGISwapChain _swapchain;
    private ID3D11Texture2D _swapchainTexture;
    private ID3D11RenderTargetView _swapchainTarget;
    
    private readonly D3D11Target _depthTarget;

    private uint _targetSwapInterval;
    private Size<int> _swapchainSize;
    
    private readonly TextureBatcher _uiBatcher;
    private readonly DeferredRenderer _deferredRenderer;
    private readonly ImGuiRenderer _imGuiRenderer;
    
    internal readonly ID3D11Device Device;
    internal readonly ID3D11DeviceContext Context;
    
    public readonly Texture WhiteTexture;

    public readonly Texture BlackTexture;

    public readonly Texture NormalTexture;

    /// <summary>
    /// The 3D <see cref="Crimson.Graphics.Camera"/> that will be used when drawing.
    /// </summary>
    public Camera Camera;

    /// <summary>
    /// Get the render area size in pixels.
    /// </summary>
    public Size<int> RenderSize => _swapchainSize;

    /// <summary>
    /// Enable/disable vertical sync.
    /// </summary>
    public bool VSync
    {
        get => _targetSwapInterval == 1;
        set => _targetSwapInterval = value ? 1u : 0u;
    }

    /// <summary>
    /// Gets the ImGUI context pointer.
    /// </summary>
    public ImGuiContextPtr ImGuiContext => _imGuiRenderer.Context;
    
    /// <summary>
    /// Create the graphics subsystem.
    /// </summary>
    /// <param name="appName">The application name.</param>
    /// <param name="info">The <see cref="SurfaceInfo"/> to use when creating the subsystem.</param>
    /// <param name="size">The size of the swapchain.</param>
    public Renderer(string appName, in SurfaceInfo info, Size<int> size)
    {
        _swapchainSize = size;
        VSync = true;

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

        Logger.Trace("Creating D3D11 device.");
        D3D11.D3D11CreateDeviceAndSwapChain(null, DriverType.Hardware, flags, levels, swapchainDesc, out _swapchain!,
            out Device!, out _, out Context!).CheckError();

        // Does not work with DXVK-native.
        if (OperatingSystem.IsWindows())
        {
            IDXGIAdapter adapter = Device.QueryInterface<IDXGIDevice>().GetAdapter();
            AdapterDescription adapterDesc = adapter.Description;

            Logger.Info("Adapter:");
            Logger.Info($"    Name: {adapterDesc.Description}");
            Logger.Info($"    Memory: {adapterDesc.DedicatedVideoMemory / 1024 / 1024}MB");
        }

        Logger.Trace("Creating swapchain textures.");
        _swapchainTexture = _swapchain.GetBuffer<ID3D11Texture2D>(0);
        _swapchainTarget = Device.CreateRenderTargetView(_swapchainTexture);
        
        _depthTarget = new D3D11Target(Device, Format.D32_Float, size, false);

        Logger.Trace("Creating texture batcher.");
        _uiBatcher = new TextureBatcher(Device);
        
        Logger.Trace("Creating deferred renderer.");
        _deferredRenderer = new DeferredRenderer(Device, size, _depthTarget);
        
        Logger.Trace("Creating ImGUI renderer.");
        _imGuiRenderer = new ImGuiRenderer(Device, RenderSize);

        Logger.Trace("Creating default textures.");
        WhiteTexture = new Texture(this, new Size<int>(1), [255, 255, 255, 255], PixelFormat.RGBA8);
        BlackTexture = new Texture(this, new Size<int>(1), [0, 0, 0, 255], PixelFormat.RGBA8);
        NormalTexture = new Texture(this, new Size<int>(1), [128, 128, 255, 255], PixelFormat.RGBA8);

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
        NormalTexture.Dispose();
        BlackTexture.Dispose();
        WhiteTexture.Dispose();
        
        _imGuiRenderer.Dispose();
        _deferredRenderer.Dispose();
        _uiBatcher.Dispose();

        _depthTarget.Dispose();
        _swapchainTarget.Dispose();
        _swapchainTexture.Dispose();
        _swapchain.Dispose();
        Context.Dispose();
        Device.Dispose();
    }

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
    /// <param name="tint">The tint to use, if any.</param>
    public void DrawImage(Texture texture, in Vector2 position, Color? tint = null)
    {
        Size<int> size = texture.Size;
        
        Vector2 topLeft = position;
        Vector2 topRight = position + new Vector2(size.Width, 0);
        Vector2 bottomLeft = position + new Vector2(0, size.Height);
        Vector2 bottomRight = position + new Vector2(size.Width, size.Height);

        _uiBatcher.AddToDrawQueue(new TextureBatcher.Draw(texture, topLeft, topRight, bottomLeft, bottomRight, tint ?? Color.White));
    }

    /// <summary>
    /// Draw an image to the screen with the given size.
    /// </summary>
    /// <param name="texture">The texture to use as the image.</param>
    /// <param name="position">The position, in pixels.</param>
    /// <param name="size">The size, in pixels.</param>
    /// <param name="tint">The tint to use, if any.</param>
    public void DrawImage(Texture texture, in Vector2 position, Size<int> size, Color? tint = null)
    {
        Vector2 topLeft = position;
        Vector2 topRight = position + new Vector2(size.Width, 0);
        Vector2 bottomLeft = position + new Vector2(0, size.Height);
        Vector2 bottomRight = position + new Vector2(size.Width, size.Height);
        
        _uiBatcher.AddToDrawQueue(new TextureBatcher.Draw(texture, topLeft, topRight, bottomLeft, bottomRight, tint ?? Color.White));
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

        Camera.Skybox?.Render();
        
        Matrix4x4 projection =
            Matrix4x4.CreateOrthographicOffCenter(0, _swapchainSize.Width, _swapchainSize.Height, 0, -1, 1);
        
        _uiBatcher.DispatchDrawQueue(Context, projection, Matrix4x4.Identity);
        
        _imGuiRenderer.Render(Context);
        
        _swapchain.Present(_targetSwapInterval);
    }
}