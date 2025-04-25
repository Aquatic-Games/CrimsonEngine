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

    private uint _targetSwapInterval;
    private Size<int> _swapchainSize;
    
    private readonly TextureBatcher _uiBatcher;
    /*private readonly DeferredRenderer _deferredRenderer;
    private readonly ImGuiRenderer _imGuiRenderer;*/

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
        get => _targetSwapInterval == 1;
        set => _targetSwapInterval = value ? 1u : 0u;
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
        VSync = true;

        _window = info.NativeHandle;

        SDL.SetAppMetadata(appName, null!, null!);

        Logger.Trace("Creating device.");
        Device = SDL.CreateGPUDevice(SDL.GPUShaderFormat.SPIRV, true, null!).Check("Create device");
        
        Logger.Trace("Claiming window for device.");
        SDL.ClaimWindowForGPUDevice(Device, _window).Check("Claim window for device");

        SDL.GPUTextureCreateInfo depthTargetInfo = new()
        {
            Type = SDL.GPUTextureType.Texturetype2D,
            Format = SDL.GPUTextureFormat.D32Float,
            Width = (uint) size.Width,
            Height = (uint) size.Height,
            LayerCountOrDepth = 1,
            Usage = SDL.GPUTextureUsageFlags.DepthStencilTarget,
            NumLevels = 1,
            SampleCount = SDL.GPUSampleCount.SampleCount1
        };

        _depthTexture = SDL.CreateGPUTexture(Device, in depthTargetInfo).Check("Create depth texture");

        MainTargetFormat = SDL.GetGPUSwapchainTextureFormat(Device, _window);

        Logger.Trace("Creating texture batcher.");
        _uiBatcher = new TextureBatcher(Device, SDL.GetGPUSwapchainTextureFormat(Device, _window));

        /*Logger.Trace("Creating deferred renderer.");
        _deferredRenderer = new DeferredRenderer(Device, size, _depthTarget);

        Logger.Trace("Creating ImGUI renderer.");
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
        
        /*_imGuiRenderer.Dispose();
        _deferredRenderer.Dispose();*/
        _uiBatcher.Dispose();

        SDL.ReleaseGPUTexture(Device, _depthTexture);
        SDL.ReleaseWindowFromGPUDevice(Device, _window);
        SDL.DestroyGPUDevice(Device);
    }

    /*/// <summary>
    /// Draw a <see cref="Renderable"/> to the screen using the built-in renderers.
    /// </summary>
    /// <param name="renderable">The <see cref="Renderable"/> to draw.</param>
    /// <param name="worldMatrix">The world matrix.</param>
    public void DrawRenderable(Renderable renderable, Matrix4x4 worldMatrix)
    {
        _deferredRenderer.AddToQueue(renderable, worldMatrix);
    }*/

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

        SDL.GPUColorTargetInfo targetInfo = new()
        {
            Texture = swapchainTexture,
            ClearColor = new SDL.FColor(0.0f, 0.0f, 0.0f, 1.0f),
            LoadOp = SDL.GPULoadOp.Load,
            StoreOp = SDL.GPUStoreOp.Store
        };
        
        //_deferredRenderer.Render(Context, _swapchainTarget, Camera.Matrices);

        Camera.Skybox?.Render(cb, swapchainTexture, _depthTexture, Camera.Matrices);
        
        Matrix4x4 projection =
            Matrix4x4.CreateOrthographicOffCenter(0, _swapchainSize.Width, _swapchainSize.Height, 0, -1, 1);
        
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
        
        //_depthTarget.Dispose();

        /*_depthTarget = new D3D11Target(Device, Format.D32_Float, newSize, false);
        
        _deferredRenderer.Resize(_depthTarget, newSize);
        _imGuiRenderer.Resize(newSize);*/
    }
}