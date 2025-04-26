using System.Diagnostics;
using System.Numerics;
using Crimson.Core;
using Crimson.Graphics.Renderers;
using Crimson.Graphics.Renderers.Structs;
//using Crimson.Graphics.Renderers;
using Crimson.Graphics.Utils;
using Crimson.Math;
using Hexa.NET.ImGui;
using SDL3;
using Color = Crimson.Math.Color;

namespace Crimson.Graphics;

/// <summary>
/// The graphics subsystem, containing everything used to render.
/// </summary>
public sealed class Renderer : IDisposable
{
    private readonly IntPtr _window;
    
    private IntPtr _depthTexture;

    private bool _vsyncEnabled;
    private Size<int> _swapchainSize;
    
    private readonly TextureBatcher _uiBatcher;
    private readonly DeferredRenderer _deferredRenderer;
    /*private readonly ImGuiRenderer _imGuiRenderer;*/

    internal readonly IntPtr Device;

    internal readonly SDL.GPUTextureFormat MainTargetFormat;
    
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
        get => _vsyncEnabled;
        set
        {
            _vsyncEnabled = value;

            SDL.SetGPUSwapchainParameters(Device, _window, SDL.GPUSwapchainComposition.SDR,
                value ? SDL.GPUPresentMode.VSync : SDL.GPUPresentMode.Immediate).Check("Set swapchain parameters");
        }
    }

    /*/// <summary>
    /// Gets the ImGUI context pointer.
    /// </summary>
    public ImGuiContextPtr ImGuiContext => _imGuiRenderer.Context;*/
    
    /// <summary>
    /// Create the graphics subsystem.
    /// </summary>
    /// <param name="appName">The application name.</param>
    /// <param name="info">The <see cref="SurfaceInfo"/> to use when creating the subsystem.</param>
    /// <param name="size">The size of the swapchain.</param>
    public Renderer(string appName, in SurfaceInfo info, Size<int> size)
    {
        _swapchainSize = size;

        _window = info.NativeHandle;

        SDL.SetAppMetadata(appName, null!, null!);

        uint props = SDL.CreateProperties();
        SDL.SetBooleanProperty(props, SDL.Props.GPUDeviceCreateDebugModeBoolean, true);
        SDL.SetBooleanProperty(props, SDL.Props.GPUDeviceCreateShadersSPIRVBoolean, true);
        
#if DEBUG
        SDL.SetBooleanProperty(props, SDL.Props.GPUDeviceCreatePreferLowPowerBoolean, true);
#endif

        if (OperatingSystem.IsWindows())
        {
            SDL.SetBooleanProperty(props, SDL.Props.GPUDeviceCreateShadersDXILBoolean, true);
            // Use D3D12 on windows
            SDL.SetStringProperty(props, SDL.Props.GPUDeviceCreateNameString, "direct3d12");
        }

        Logger.Trace("Creating device.");
        Device = SDL.CreateGPUDeviceWithProperties(props).Check("Create device");
        
        Console.WriteLine($"Using SDL backend: {SDL.GetGPUDeviceDriver(Device)}");
        
        Logger.Trace("Claiming window for device.");
        SDL.ClaimWindowForGPUDevice(Device, _window).Check("Claim window for device");
        
        VSync = true;

        _depthTexture = SdlUtils.CreateTexture2D(Device, (uint) size.Width, (uint) size.Height,
            SDL.GPUTextureFormat.D32Float, 1, SDL.GPUTextureUsageFlags.DepthStencilTarget);

        MainTargetFormat = SDL.GetGPUSwapchainTextureFormat(Device, _window);

        Logger.Trace("Creating texture batcher.");
        _uiBatcher = new TextureBatcher(Device, SDL.GetGPUSwapchainTextureFormat(Device, _window));

        Logger.Trace("Creating deferred renderer.");
        _deferredRenderer = new DeferredRenderer(Device, size, MainTargetFormat);

        /*Logger.Trace("Creating ImGUI renderer.");
        _imGuiRenderer = new ImGuiRenderer(Device, RenderSize);*/

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
        
        /*_imGuiRenderer.Dispose();*/
        _deferredRenderer.Dispose();
        _uiBatcher.Dispose();

        SDL.ReleaseGPUTexture(Device, _depthTexture);
        SDL.ReleaseWindowFromGPUDevice(Device, _window);
        SDL.DestroyGPUDevice(Device);
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
    public unsafe void Render()
    {
        IntPtr cb = SDL.AcquireGPUCommandBuffer(Device).Check("Acquire command buffer");

        SDL.WaitAndAcquireGPUSwapchainTexture(cb, _window, out IntPtr swapchainTexture, out _, out _)
            .Check("Acquire swapchain texture");

        if (swapchainTexture == IntPtr.Zero)
            return;
        
        _deferredRenderer.Render(cb, swapchainTexture, _depthTexture, Camera.Matrices);

        Camera.Skybox?.Render(cb, swapchainTexture, _depthTexture, Camera.Matrices);
        
        Matrix4x4 projection =
            Matrix4x4.CreateOrthographicOffCenter(0, _swapchainSize.Width, _swapchainSize.Height, 0, -1, 1);
        
        SDL.GPUColorTargetInfo targetInfo = new()
        {
            Texture = swapchainTexture,
            ClearColor = new SDL.FColor(0.0f, 0.0f, 0.0f, 1.0f),
            LoadOp = SDL.GPULoadOp.Load,
            StoreOp = SDL.GPUStoreOp.Store
        };
        
        _uiBatcher.DispatchDrawQueue(cb, targetInfo, _swapchainSize, new CameraMatrices(projection, Matrix4x4.Identity));
        
        //_imGuiRenderer.Render(Context);

        SDL.SubmitGPUCommandBuffer(cb);
    }

    /// <summary>
    /// Resize the renderer.
    /// </summary>
    /// <param name="newSize">The new size to set.</param>
    public void Resize(Size<int> newSize)
    {
        _swapchainSize = newSize;
        
        SDL.ReleaseGPUTexture(Device, _depthTexture);

        _depthTexture = SdlUtils.CreateTexture2D(Device, (uint) newSize.Width, (uint) newSize.Height,
            SDL.GPUTextureFormat.D32Float, 1, SDL.GPUTextureUsageFlags.DepthStencilTarget);
        
        _deferredRenderer.Resize(newSize);
        /*_imGuiRenderer.Resize(newSize);*/
    }
}