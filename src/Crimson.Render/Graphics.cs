using System.Numerics;
using Crimson.Core;
using Crimson.Math;
using Crimson.Render.Renderers;
using grabs.Core;
using grabs.Graphics;
using grabs.Graphics.Vulkan;

namespace Crimson.Render;

/// <summary>
/// The graphics subsystem, containing everything used to render.
/// </summary>
public sealed class Graphics : IDisposable
{
    private readonly Instance _instance;
    private readonly Surface _surface;
    private readonly Swapchain _swapchain;

    private uint _targetSwapInterval;
    private Size<int> _swapchainSize;
    
    private readonly TextureBatcher _uiBatcher;
    /*private readonly DeferredRenderer _deferredRenderer;*/
    
    internal readonly Device Device;
    internal readonly CommandList CommandList;
    
    public readonly Texture WhiteTexture;

    public readonly Texture BlackTexture;

    public readonly Texture NormalTexture;

    /// <summary>
    /// The 3D <see cref="Crimson.Render.Camera"/> that will be used when drawing.
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
    /// Create the graphics subsystem.
    /// </summary>
    /// <param name="appName">The application name.</param>
    /// <param name="info">The <see cref="SurfaceInfo"/> to use when creating the subsystem.</param>
    /// <param name="size">The size of the swapchain.</param>
    public Graphics(string appName, in SurfaceInfo info, Size<int> size)
    {
        _swapchainSize = size;
        VSync = true;

        Instance.RegisterBackend<VulkanBackend>();

        InstanceInfo instanceInfo = new InstanceInfo(appName, true);

        _instance = Instance.Create(in instanceInfo);
        _surface = _instance.CreateSurface(in info);

        Device = _instance.CreateDevice(_surface);
        CommandList = Device.CreateCommandList();
        
        Logger.Info("Adapter:");
        Logger.Info($"    Name: {Device.Adapter.Name}");
        Logger.Info($"    Memory: {Device.Adapter.DedicatedMemory / 1024 / 1024}MB");

        SwapchainInfo swapchainInfo = new()
        {
            Surface = _surface,
            Size = new Size2D((uint) size.Width, (uint) size.Height),
            Format = Format.B8G8R8A8_UNorm,
            NumBuffers = 2,
            PresentMode = PresentMode.Fifo
        };

        _swapchain = Device.CreateSwapchain(in swapchainInfo);

        Logger.Trace("Creating texture batcher.");
        _uiBatcher = new TextureBatcher(Device, _swapchain.SwapchainFormat);
        
        /*Logger.Trace("Creating deferred renderer.");
        _deferredRenderer = new DeferredRenderer(Device, size);*/

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
        
        /*_deferredRenderer.Dispose();*/
        _uiBatcher.Dispose();
        
        _swapchain.Dispose();
        CommandList.Dispose();
        Device.Dispose();
        _surface.Dispose();
        _instance.Dispose();
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
        GrabsTexture swapchainTexture = _swapchain.GetNextTexture();
        
        CommandList.Begin();
        CommandList.SetViewport(new Viewport(0, 0, _swapchainSize.Width, _swapchainSize.Height));
        
        CommandList.BeginRenderPass(new RenderPassInfo(new ColorAttachmentInfo(swapchainTexture, new ColorF(1.0f, 0.5f, 0.25f))));
        CommandList.EndRenderPass();

        RenderPassInfo uiPass = new()
        {
            ColorAttachments =
            [
                new ColorAttachmentInfo()
                {
                    Texture = swapchainTexture,
                    LoadOp = LoadOp.Load
                }
            ]
        };
        CommandList.BeginRenderPass(in uiPass);
        
        Matrix4x4 projection =
            Matrix4x4.CreateOrthographicOffCenter(0, _swapchainSize.Width, _swapchainSize.Height, 0, -1, 1);
        
        _uiBatcher.DispatchDrawQueue(CommandList, projection, Matrix4x4.Identity);
        
        CommandList.EndRenderPass();
        CommandList.End();
        
        Device.ExecuteCommandList(CommandList);
        
        _swapchain.Present();
    }
}