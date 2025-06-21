using System.Diagnostics;
using Crimson.Core;
using Crimson.Engine.Entities;
using Crimson.Graphics;
using Crimson.Math;
using Crimson.Physics;
using Crimson.Platform;

namespace Crimson.Engine;

/// <summary>
/// The main application instance that runs the Crimson engine.
/// </summary>
public static class App
{
    private static string _appName;
    private static bool _isRunning;
    private static GlobalApp _globalApp;

    private static Stopwatch _deltaWatch;
    private static double _targetDelta;
    
    private static Scene _currentScene;
    private static Scene? _switchScene;
    
    private static Renderer _renderer;
    private static PhysicsSystem _physics;

    private static ImGuiController? _imGuiController;
    
    /// <summary>
    /// The app name.
    /// </summary>
    public static string Name => _appName;
    
    /// <summary>
    /// Is the app running? If false, the subsystems may not be initialized.
    /// You should only use engine features when this value is true.
    /// </summary>
    public static bool IsRunning => _isRunning;

    /// <summary>
    /// Set the FPS limit for the application. A value of 0 will disable the limit, however the FPS may still be limited
    /// by other factors (VSync, Compositor, etc.)
    /// </summary>
    public static uint FpsLimit
    {
        get => (uint) (1.0 / _targetDelta);
        set => _targetDelta = value == 0 ? 0 : 1.0 / value;
    }

    /// <summary>
    /// The global application instance.
    /// </summary>
    public static GlobalApp GlobalApp => _globalApp;
    
    /// <summary>
    /// Get the currently active <see cref="Scene"/>.
    /// </summary>
    public static Scene ActiveScene => _currentScene;

    /// <summary>
    /// The app's <see cref="Renderer"/> instance.
    /// </summary>
    public static Renderer Renderer => _renderer;

    /// <summary>
    /// The physics system.
    /// </summary>
    public static PhysicsSystem Physics => _physics;

    static App()
    {
        _appName = "";
        _isRunning = false;
        _globalApp = null!;
        _renderer = null!;
        _currentScene = null!;
        _deltaWatch = null!;
        _physics = null!;
        _imGuiController = null!;
        FpsLimit = 0;
    }
    
    /// <summary>
    /// Run the application.
    /// </summary>
    /// <param name="options">The <see cref="AppOptions"/> to use on startup.</param>
    /// <param name="scene">The initial scene to load.</param>
    /// <param name="globalApp">A <see cref="Crimson.Engine.GlobalApp"/> instance, if any.</param>
    public static void Run(in AppOptions options, Scene scene, GlobalApp? globalApp = null)
    {
        Debug.Assert(_isRunning == false);
        
        Logger.Info("Crimson 1.0.0");
        Logger.Info("Starting app:");
        Logger.Info($"    Name: {options.Name}");
        Logger.Info($"    Version: {options.Version}");
        
        _appName = options.Name;
        _globalApp = globalApp ?? new GlobalApp();
        _currentScene = scene;
        
        Logger.Debug("Initializing events manager.");
        Events.Create();
        
        Logger.Debug("Creating surface.");
        Surface.Create(in options.Window);
        Events.WindowClose += Close;
        Events.SurfaceSizeChanged += OnSurfaceSizeChanged;
        
        Logger.Debug("Initializing graphics subsystem.");
        _renderer = new Renderer(_appName, in options.Renderer, Surface.Info);
        
        Logger.Debug("Initializing input manager.");
        Input.Create();
        
        Logger.Debug("Initializing physics system.");
        _physics = new PhysicsSystem();

        if (_renderer.ImGuiContext is { } context)
        {
            Logger.Debug("Creating imgui controller.");
            _imGuiController = new ImGuiController(context);
        }

        _deltaWatch = Stopwatch.StartNew();
        
        _isRunning = true;
        
        Logger.Debug("Initializing user code.");
        _globalApp.Initialize();
        _currentScene.Initialize();

        Logger.Debug("Entering main loop.");
        while (_isRunning)
        {
            if (_deltaWatch.Elapsed.TotalSeconds < _targetDelta && !Renderer.VSync)
                continue;
            
            Input.Update();
            Events.ProcessEvents();

            float dt = (float) _deltaWatch.Elapsed.TotalSeconds;
            _deltaWatch.Restart();

            if (_switchScene != null)
            {
                _currentScene.Dispose();
                _currentScene = _switchScene;
                _switchScene = null;
                GC.Collect();
                _currentScene.Initialize();
            }
            
            _imGuiController?.Update(dt);
            
            _globalApp.PreUpdate(dt);
            _physics.Step(1 / 60.0f);
            _currentScene.Update(dt);
            _globalApp.PostUpdate(dt);
            
            _renderer.NewFrame();
            
            _globalApp.PreDraw();
            _currentScene.Draw();
            _globalApp.PostDraw();
            
            _renderer.Render();
        }
        
        Logger.Info("Cleaning up.");
        
        _currentScene.Dispose();
        _globalApp.Dispose();
        _physics.Dispose();
        Input.Destroy();
        _renderer.Dispose();
        Surface.Destroy();
        Events.Destroy();
    }

    /// <summary>
    /// Close the app.
    /// </summary>
    public static void Close()
    {
        Debug.Assert(_isRunning == true);
        
        _isRunning = false;
    }

    public static void SetScene(Scene scene)
    {
        _switchScene = scene;
    }
    
    private static void OnSurfaceSizeChanged(Size<int> newSize)
    {
        _renderer.Resize(newSize);
    }
}