using System.Diagnostics;
using Crimson.Core;
using Crimson.Graphics.Utils;
using Crimson.Math;
using Graphite;
using Graphite.Core;
using Graphite.D3D11;
using Graphite.Vulkan;
using Hexa.NET.ImGui;

namespace Crimson.Graphics;

/// <summary>
/// The graphics subsystem, containing everything used to render.
/// </summary>
public static class Renderer
{
    private static Instance _instance;
    private static Surface _surface;
    private static Swapchain _swapchain;

    internal static Device Device;
    internal static CommandList CommandList;

    public static string Backend => _instance.BackendName;

    /// <summary>
    /// The 3D <see cref="Crimson.Graphics.Camera"/> that will be used when drawing.
    /// </summary>
    public static Camera Camera;

    /// <summary>
    /// Get the render area size in pixels.
    /// </summary>
    public static Size<int> RenderSize => _swapchain.Size.ToCrimson();

    /// <summary>
    /// Enable/disable vertical sync.
    /// </summary>
    public static bool VSync
    {
        get => true;
        set => throw new NotImplementedException();
    }
    /*public static bool VSync
    {
        get => _vsyncEnabled;
        set
        {
            _vsyncEnabled = value;

            SDL.SetGPUSwapchainParameters(Device, _window, SDL.GPUSwapchainComposition.SDR,
                value ? SDL.GPUPresentMode.VSync : SDL.GPUPresentMode.Immediate).Check("Set swapchain parameters");
        }
    }*/

    /*/// <summary>
    /// Gets the ImGUI context pointer.
    /// </summary>
    public static ImGuiContextPtr? ImGuiContext
    {
        get
        {
            Debug.Assert(_imGuiRenderer != null, "Renderer has not been created with CreateImGuiRenderer enabled.");
            return _imGuiRenderer.Context;
        }
    }

    /// <summary>
    /// The main renderer debug textures.
    /// </summary>
    public static Texture[] DebugRendererTextures => _deferredRenderer!.DebugTextures;*/
    
    /// <summary>
    /// Create the graphics subsystem.
    /// </summary>
    /// <param name="appName">The application name.</param>
    /// <param name="surface">The <see cref="SurfaceInfo"/> to use when creating the subsystem.</param>
    public static void Create(string appName, in RendererOptions options, in SurfaceInfo surface, in Size<int> size)
    {
        GraphiteLog.LogMessage += OnLog;
        
        Instance.RegisterBackend<D3D11Backend>();
        Instance.RegisterBackend<VulkanBackend>();

        InstanceInfo instanceInfo = new()
        {
            AppName = appName,
            Debug = options.Debug
        };
        
        Logger.Trace("Creating instance.");
        _instance = Instance.Create(in instanceInfo);
        
        Logger.Trace("Creating surface.");
        _surface = _instance.CreateSurface(in surface);
        
        Logger.Trace("Creating device.");
        Device = _instance.CreateDevice(_surface);

        SwapchainInfo swapchainInfo = new()
        {
            Surface = _surface,
            Format = Format.B8G8R8A8_UNorm,
            Size = size.ToGraphite(),
            NumBuffers = 2,
            PresentMode = PresentMode.Fifo
        };
        
        Logger.Trace("Creating swapchain.");
        _swapchain = Device.CreateSwapchain(in swapchainInfo);

        Logger.Trace("Creating command list.");
        CommandList = Device.CreateCommandList();
    }

    /// <summary>
    /// Destroy the graphics subsystem.
    /// </summary>
    public static void Destroy()
    {
        CommandList.Dispose();
        _swapchain.Dispose();
        Device.Dispose();
        _surface.Dispose();
        _instance.Dispose();

        GraphiteLog.LogMessage -= OnLog;
    }

    /*/// <summary>
    /// Draw a <see cref="Renderable"/> to the screen using the built-in renderers.
    /// </summary>
    /// <param name="renderable">The <see cref="Renderable"/> to draw.</param>
    /// <param name="worldMatrix">The world matrix.</param>
    public static void DrawRenderable(Renderable renderable, Matrix4x4 worldMatrix)
    {
        Debug.Assert(_deferredRenderer != null, "Renderer has not been created with 3D rendering enabled.");
        _deferredRenderer.AddToQueue(renderable, worldMatrix);
    }

    /// <summary>
    /// Draw a <see cref="Sprite"/> to the screen using the built-in renderers.
    /// </summary>
    /// <param name="sprite">The sprite to draw.</param>
    /// <param name="matrix">The matrix.</param>
    /// <remarks>Set the sprite's color to red to see just how deep the rabbit hole goes.</remarks>
    public static void DrawSprite(Sprite sprite, Matrix<float> matrix)
    {
        Debug.Assert(_spriteRenderer != null, "Renderer has not been created with 2D rendering enabled.");
        _spriteRenderer.DrawSprite(in sprite, matrix);
    }
    
    /// <summary>
    /// Draw an image to the screen with the given size.
    /// </summary>
    /// <param name="texture">The texture to use as the image.</param>
    /// <param name="position">The position, in pixels.</param>
    /// <param name="size">The size, in pixels.</param>
    /// <param name="source">The source rectangle to use, if any.</param>
    /// <param name="tint">The tint to use, if any.</param>
    /// <param name="blend">The blending mode to use on draw.</param>
    public static void DrawImage(Texture texture, Vector2T<int> position, Size<int> size, Rectangle<int>? source = null,
        Color? tint = null, BlendMode blend = BlendMode.Blend)
    {
        Rectangle<int> src = source ?? new Rectangle<int>(Vector2T<int>.Zero, texture.Size);
        
        Vector2T<int> topLeft = position;
        Vector2T<int> topRight = position + new Vector2T<int>(size.Width, 0);
        Vector2T<int> bottomLeft = position + new Vector2T<int>(0, size.Height);
        Vector2T<int> bottomRight = position + new Vector2T<int>(size.Width, size.Height);

        _uiBatcher.AddToDrawQueue(new TextureBatcher.Draw(texture, topLeft.As<float>(), topRight.As<float>(),
            bottomLeft.As<float>(), bottomRight.As<float>(), src, tint ?? Color.White, blend));
    }

    /// <summary>
    /// Draw an image to the screen.
    /// </summary>
    /// <param name="texture">The texture to use as the image.</param>
    /// <param name="position">The position, in pixels.</param>
    /// <param name="source">The source rectangle to use, if any.</param>
    /// <param name="tint">The tint to use, if any.</param>
    /// <param name="blend">The blending mode to use on draw.</param>
    public static void DrawImage(Texture texture, Vector2T<int> position, Rectangle<int>? source = null,
        Color? tint = null, BlendMode blend = BlendMode.Blend)
    {
        DrawImage(texture, position, source?.Size ?? texture.Size, source, tint, blend);
    }

    /// <summary>
    /// Draw text to the screen.
    /// </summary>
    /// <param name="font">The <see cref="Font"/> to use.</param>
    /// <param name="position">The position, in pixels.</param>
    /// <param name="size">The font size, in pixels.</param>
    /// <param name="text">The text to draw.</param>
    /// <param name="color">The text's color.</param>
    public static void DrawText(Font font, Vector2T<int> position, uint size, string text, Color color)
    {
        font.Draw(_uiBatcher, position, size, text, color);
    }

    /// <summary>
    /// Draw a line from a to b.
    /// </summary>
    /// <param name="a">The first point.</param>
    /// <param name="b">The second point.</param>
    /// <param name="color">The line color.</param>
    /// <param name="thickness">The line thickness in pixels.</param>
    public static void DrawLine(in Vector2T<int> a, in Vector2T<int> b, in Color color, int thickness)
    {
        float halfThickness = thickness / 2.0f;

        Vector2T<float> topLeft;
        Vector2T<float> bottomLeft;
        Vector2T<float> topRight;
        Vector2T<float> bottomRight;

        // Use fast paths if drawing hline or vline as it avoids the complex matrix calculations
        if (a.X == b.X)
        {
            topLeft = new Vector2T<float>(a.X - halfThickness, a.Y);
            topRight = new Vector2T<float>(a.X + halfThickness, a.Y);
            bottomLeft = new Vector2T<float>(b.X - halfThickness, b.Y);
            bottomRight = new Vector2T<float>(b.X + halfThickness, b.Y);
        }
        else if (a.Y == b.Y)
        {
            topLeft = new Vector2T<float>(a.X, a.Y - halfThickness);
            bottomLeft = new Vector2T<float>(a.X, a.Y + halfThickness);
            topRight = new Vector2T<float>(b.X, b.Y - halfThickness);
            bottomRight = new Vector2T<float>(b.X, b.Y + halfThickness);
        }
        else
        {
            Vector2T<float> fA = a.As<float>();
            Vector2T<float> fB = b.As<float>();
            
            float rot = float.Atan2(b.Y - a.Y, b.X - a.X);
            float dist = Vector2T.Distance(fA, fB);

            Matrix<float> rotMatrix = Matrix.RotateZ(rot);

            topLeft = Vector2T.Transform(new Vector2T<float>(0, -halfThickness), rotMatrix) + fA;
            bottomLeft = Vector2T.Transform(new Vector2T<float>(0, +halfThickness), rotMatrix) + fA;
            topRight = Vector2T.Transform(new Vector2T<float>(dist, -halfThickness), rotMatrix) + fA;
            bottomRight = Vector2T.Transform(new Vector2T<float>(dist, +halfThickness), rotMatrix) + fA;
        }

        _uiBatcher.AddToDrawQueue(new TextureBatcher.Draw(Texture.White, topLeft, topRight, bottomLeft, bottomRight,
            new Rectangle<int>(0, 0, 1, 1), color, BlendMode.Blend));
    }

    public static void DrawFilledRectangle(Vector2T<int> position, Size<int> size, Color color)
    {
        Vector2T<int> topLeft = position;
        Vector2T<int> topRight = new Vector2T<int>(position.X + size.Width, position.Y);
        Vector2T<int> bottomLeft = new Vector2T<int>(position.X, position.Y + size.Height);
        Vector2T<int> bottomRight = new Vector2T<int>(position.X + size.Width, position.Y + size.Height);

        _uiBatcher.AddToDrawQueue(new TextureBatcher.Draw(Texture.White, topLeft.As<float>(), topRight.As<float>(),
            bottomLeft.As<float>(), bottomRight.As<float>(), new Rectangle<int>(0, 0, 1, 1), color, BlendMode.Blend));
    }

    public static void DrawBorderRectangle(Vector2T<int> position, Size<int> size, int borderWidth, Color color)
    {
        Size<int> hSize = new Size<int>(size.Width, borderWidth);
        Size<int> vSize = new Size<int>(borderWidth, size.Height);
        
        // Top
        DrawFilledRectangle(position, hSize, color);
        // Left
        DrawFilledRectangle(position, vSize, color);
        // Bottom
        DrawFilledRectangle(position + new Vector2T<int>(0, size.Height - borderWidth), hSize, color);
        // Right
        DrawFilledRectangle(position + new Vector2T<int>(size.Width - borderWidth, 0), vSize, color);
    }

    public static void DrawRectangle(Vector2T<int> position, Size<int> size, Color fillColor, int borderWidth,
        Color borderColor)
    {
        DrawFilledRectangle(position, size, fillColor);
        DrawBorderRectangle(position, size, borderWidth, borderColor);
    }

    public static void NewFrame()
    {
        Metrics.BeginPerformanceMetric(Metrics.RenderTimeMetric);
        
        Camera = default;
    }*/

    /// <summary>
    /// Render and present to the surface.
    /// </summary>
    public static void Render()
    {
        GrTexture swapchainTexture = _swapchain.GetNextTexture();
        
        
        
        _swapchain.Present();
    }

    /// <summary>
    /// Resize the renderer.
    /// </summary>
    /// <param name="newSize">The new size to set.</param>
    public static void Resize(Size<int> newSize)
    {
        
    }
    
    private static void OnLog(GraphiteLog.Severity severity, GraphiteLog.Type type, string message, int line, string file)
    {
        Logger.Severity cSeverity = severity switch
        {
            GraphiteLog.Severity.Verbose => Logger.Severity.Trace,
            GraphiteLog.Severity.Info => Logger.Severity.Debug,
            GraphiteLog.Severity.Warning => Logger.Severity.Warning,
            GraphiteLog.Severity.Error => Logger.Severity.Error,
            _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
        };
        
        Logger.Log(cSeverity, $"{type}: {message}", line, file);
    }
}