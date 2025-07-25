﻿using System.Diagnostics;
using System.Numerics;
using Crimson.Core;
using Crimson.Graphics.Renderers;
using Crimson.Graphics.Renderers.Structs;
using Crimson.Graphics.Utils;
using Crimson.Math;
using Hexa.NET.ImGui;
using SDL3;
using Color = Crimson.Math.Color;

namespace Crimson.Graphics;

/// <summary>
/// The graphics subsystem, containing everything used to render.
/// </summary>
public static class Renderer
{
    private static IntPtr _window;
    
    private static IntPtr _depthTexture;

    private static bool _vsyncEnabled;
    private static Size<int> _swapchainSize;
    
    private static TextureBatcher _uiBatcher;
    private static ImGuiRenderer? _imGuiRenderer;
    private static DeferredRenderer? _deferredRenderer;
    private static SpriteRenderer? _spriteRenderer;

    internal static IntPtr Device;

    internal static SDL.GPUTextureFormat MainTargetFormat;

    public static string Backend => SDL.GetGPUDeviceDriver(Device) ?? "Unknown";

    /// <summary>
    /// The 3D <see cref="Crimson.Graphics.Camera"/> that will be used when drawing.
    /// </summary>
    public static Camera Camera;

    /// <summary>
    /// Get the render area size in pixels.
    /// </summary>
    public static Size<int> RenderSize => _swapchainSize;

    /// <summary>
    /// Enable/disable vertical sync.
    /// </summary>
    public static bool VSync
    {
        get => _vsyncEnabled;
        set
        {
            _vsyncEnabled = value;

            SDL.SetGPUSwapchainParameters(Device, _window, SDL.GPUSwapchainComposition.SDR,
                value ? SDL.GPUPresentMode.VSync : SDL.GPUPresentMode.Immediate).Check("Set swapchain parameters");
        }
    }

    /// <summary>
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
    public static Texture[] DebugRendererTextures => _deferredRenderer!.DebugTextures;
    
    /// <summary>
    /// Create the graphics subsystem.
    /// </summary>
    /// <param name="appName">The application name.</param>
    /// <param name="surface">The <see cref="SurfaceInfo"/> to use when creating the subsystem.</param>
    public static void Create(string appName, in RendererOptions options, in SurfaceInfo surface)
    {
        _swapchainSize = surface.Size;

        _window = surface.Handle;

        SDL.SetAppMetadata(appName, null!, null!);

        uint props = SDL.CreateProperties();
        SDL.SetBooleanProperty(props, SDL.Props.GPUDeviceCreateShadersSPIRVBoolean, true);

        if (options.Debug)
        {
            SDL.SetBooleanProperty(props, SDL.Props.GPUDeviceCreateDebugModeBoolean, true);
            SDL.SetBooleanProperty(props, SDL.Props.GPUDeviceCreatePreferLowPowerBoolean, true);
        }

        if (OperatingSystem.IsWindows() && !EnvVar.IsTrue(EnvVar.ForceVulkan))
        {
            SDL.SetBooleanProperty(props, SDL.Props.GPUDeviceCreateShadersDXILBoolean, true);
            // Use D3D12 on windows
            SDL.SetStringProperty(props, SDL.Props.GPUDeviceCreateNameString, "direct3d12");
        }

        Logger.Trace("Creating device.");
        Device = SDL.CreateGPUDeviceWithProperties(props).Check("Create device");
        
        Logger.Debug($"Using SDL backend: {SDL.GetGPUDeviceDriver(Device)}");
        
        Logger.Trace("Claiming window for device.");
        SDL.ClaimWindowForGPUDevice(Device, _window).Check("Claim window for device");
        
        VSync = true;

        _depthTexture = SdlUtils.CreateTexture2D(Device, (uint) _swapchainSize.Width, (uint) _swapchainSize.Height,
            SDL.GPUTextureFormat.D32Float, 1, SDL.GPUTextureUsageFlags.DepthStencilTarget);

        MainTargetFormat = SDL.GetGPUSwapchainTextureFormat(Device, _window);

        Logger.Debug($"options.Type: {options.Type}");
        Logger.Debug($"options.CreateImGuiRenderer: {options.CreateImGuiRenderer}");
        
        Logger.Trace("Creating UI renderer.");
        _uiBatcher = new TextureBatcher(Device, SDL.GetGPUSwapchainTextureFormat(Device, _window));

        if (options.CreateImGuiRenderer)
        {
            Logger.Trace("Creating ImGUI renderer.");
            _imGuiRenderer = new ImGuiRenderer(Device, RenderSize, MainTargetFormat);
        }

        if ((options.Type & RendererType.Create3D) != 0)
        {
            Logger.Trace("Creating deferred 3D renderer.");
            _deferredRenderer = new DeferredRenderer(Device, _swapchainSize, MainTargetFormat);
        }

        if ((options.Type & RendererType.Create2D) != 0)
        {
            Logger.Trace("Creating sprite renderer.");
            _spriteRenderer = new SpriteRenderer(Device, MainTargetFormat);
        }

        Logger.Trace("Creating default textures.");
        Texture.White = new Texture(new Size<int>(1), [255, 255, 255, 255], PixelFormat.RGBA8);
        Texture.Black = new Texture(new Size<int>(1), [0, 0, 0, 255], PixelFormat.RGBA8);
        Texture.EmptyNormal = new Texture(new Size<int>(1), [128, 128, 255, 255], PixelFormat.RGBA8);

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
    public static void Destroy()
    {
        Texture.EmptyNormal.Dispose();
        Texture.EmptyNormal = null!;
        Texture.Black.Dispose();
        Texture.Black = null!;
        Texture.White.Dispose();
        Texture.White = null!;
        
        _deferredRenderer?.Dispose();
        _imGuiRenderer?.Dispose();
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
    }

    /// <summary>
    /// Render and present to the surface.
    /// </summary>
    public static void Render()
    {
        IntPtr cb = SDL.AcquireGPUCommandBuffer(Device).Check("Acquire command buffer");

        SDL.WaitAndAcquireGPUSwapchainTexture(cb, _window, out IntPtr swapchainTexture, out _, out _)
            .Check("Acquire swapchain texture");

        if (swapchainTexture == IntPtr.Zero)
            return;
        
        // Each Render() method returns a boolean. If true, it means the renderer has cleared the target provided as the
        // color/compositeTarget parameter. Each renderer has the ability to clear the render/depth targets (if applicable)
        // if necessary.
        // This acts as a small optimization. Each renderer will not bother rendering if there is nothing to do.

        bool hasCleared = _deferredRenderer?.Render(cb, swapchainTexture, _depthTexture, Camera.Matrices) ?? false;

        if (Camera.Skybox?.Render(cb, swapchainTexture, _depthTexture, !hasCleared, Camera.Matrices) ?? false)
            hasCleared = true;

        if (_spriteRenderer?.Render(cb, swapchainTexture, !hasCleared, _swapchainSize, Camera.Matrices) ?? false)
            hasCleared = true;

        Matrix4x4 projection =
            Matrix4x4.CreateOrthographicOffCenter(0, _swapchainSize.Width, _swapchainSize.Height, 0, -1, 1);

        if (_uiBatcher.Render(cb, swapchainTexture, !hasCleared, _swapchainSize,
                new CameraMatrices(projection, Matrix4x4.Identity)))
        {
            hasCleared = true;
        }

        _imGuiRenderer?.Render(cb, swapchainTexture, !hasCleared);
        
        // We have to end the metric before submitting the command buffer, as SDL will auto present when this method
        // is called, which will skew the metrics (especially if VSync is enabled.)
        // When Graphite is implemented, we'll move this to just before the present.
        Metrics.EndPerformanceMetric(Metrics.RenderTimeMetric);

        SDL.SubmitGPUCommandBuffer(cb).Check("Submit command buffer");
    }

    /// <summary>
    /// Resize the renderer.
    /// </summary>
    /// <param name="newSize">The new size to set.</param>
    public static void Resize(Size<int> newSize)
    {
        _swapchainSize = newSize;
        
        SDL.ReleaseGPUTexture(Device, _depthTexture);

        _depthTexture = SdlUtils.CreateTexture2D(Device, (uint) newSize.Width, (uint) newSize.Height,
            SDL.GPUTextureFormat.D32Float, 1, SDL.GPUTextureUsageFlags.DepthStencilTarget);
        
        _deferredRenderer?.Resize(newSize);
        _imGuiRenderer?.Resize(newSize);
    }
}