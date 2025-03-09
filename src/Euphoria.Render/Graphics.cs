using System.Diagnostics;
using System.Numerics;
using Euphoria.Core;
using Euphoria.Math;
using Euphoria.Render.Renderers;
using grabs.Core;
using grabs.Graphics;
using grabs.Graphics.D3D11;
using grabs.Graphics.Vulkan;

namespace Euphoria.Render;

/// <summary>
/// The graphics subsystem, containing everything used to render.
/// </summary>
public static class Graphics
{
    private static Surface _surface = null!;
    private static Swapchain _swapchain = null!;
    
    internal static Instance Instance = null!;
    internal static Device Device = null!;
    internal static CommandList CommandList = null!;

    private static TextureBatcher _test;
    private static Texture _texture;
    
    /// <summary>
    /// Create the graphics subsystem.
    /// </summary>
    /// <param name="info">The <see cref="SurfaceInfo"/> to use when creating the subsystem.</param>
    /// <param name="size">The size of the swapchain.</param>
    public static void Create(in SurfaceInfo info, Size<int> size)
    {
        Debug.Assert(Instance == null);
        
        GrabsLog.LogMessage += (severity, type, message, file, line) =>
        {
            Logger.Severity eSeverity = severity switch
            {
                GrabsLog.Severity.Verbose => Logger.Severity.Trace,
                GrabsLog.Severity.Info => Logger.Severity.Debug,
                GrabsLog.Severity.Warning => Logger.Severity.Warning,
                GrabsLog.Severity.Error => Logger.Severity.Error,
                _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
            };

            Logger.Log(eSeverity, $"{type}: {message}", line, file);
        };
        
        // TODO: Enable D3D11 when it actually works properly
        //if (OperatingSystem.IsWindows())
        //    Instance.RegisterBackend<D3D11Backend>();
        Instance.RegisterBackend<VulkanBackend>();

        InstanceInfo instanceInfo = new InstanceInfo("Euphoria", true);

        Instance = Instance.Create(in instanceInfo);
        _surface = Instance.CreateSurface(in info);
        Device = Instance.CreateDevice(_surface);

        SwapchainInfo swapchainInfo = new SwapchainInfo()
        {
            Surface = _surface,
            Size = new Size2D((uint) size.Width, (uint) size.Height),
            Format = _surface.GetOptimalSwapchainFormat(Device.Adapter),
            PresentMode = PresentMode.Fifo,
            NumBuffers = 2
        };
        _swapchain = Device.CreateSwapchain(in swapchainInfo);

        CommandList = Device.CreateCommandList();

        _test = new TextureBatcher(Device, _swapchain.SwapchainFormat);
        _texture = new Texture("/home/aqua/Pictures/DEBUG.png");
    }

    /// <summary>
    /// Destroy the graphics subsystem.
    /// </summary>
    public static void Destroy()
    {
        Debug.Assert(Instance != null);
        
        _texture.Dispose();
        _test.Dispose();
        CommandList.Dispose();
        _swapchain.Dispose();
        Device.Dispose();
        _surface.Dispose();
        Instance.Dispose();
    }

    /// <summary>
    /// Render and present to the surface.
    /// </summary>
    public static void Render()
    {
        GrabsTexture swapchainTexture = _swapchain.GetNextTexture();
        
        CommandList.Begin();
        CommandList.SetViewport(new Viewport(0, 0, 1280, 720));

        RenderPassInfo passInfo = new()
        {
            ColorAttachments = [new ColorAttachmentInfo(swapchainTexture, new ColorF(1.0f, 0.5f, 0.25f))]
        };
        CommandList.BeginRenderPass(in passInfo);
        
        _test.Draw(_texture, new Vector2(0, 0), new Vector2(100, 0), new Vector2(0, 100), new Vector2(100, 100));
        _test.DispatchDrawQueue(CommandList, Matrix4x4.CreateOrthographicOffCenter(0, 1280, 720, 0, -1, 1), Matrix4x4.Identity);
        
        CommandList.EndRenderPass();
        
        CommandList.End();
        
        Device.ExecuteCommandList(CommandList);
        _swapchain.Present();
    }
}