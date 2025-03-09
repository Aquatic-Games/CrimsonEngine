using System.Diagnostics;
using Euphoria.Core;
using Euphoria.Windowing;

namespace Euphoria.Engine;

public static class App
{
    private static string _appName;
    private static bool _isRunning;

    public static string Name => _appName;
    
    public static bool IsRunning => _isRunning;

    static App()
    {
        _appName = "";
        _isRunning = false;
    }
    
    public static void Run(in AppOptions options)
    {
        Debug.Assert(_isRunning == false);
        
        Logger.Info("Euphoria 1.0.0");
        Logger.Info("Starting app:");
        Logger.Info($"    Name: {options.Name}");
        Logger.Info($"    Version: {options.Version}");
        
        _appName = options.Name;
        
        Logger.Debug("Creating window.");
        Window.Create(in options.Window);
        Events.WindowClose += Close;
        
        _isRunning = true;

        Logger.Debug("Entering main loop.");
        while (_isRunning)
        {
            Events.ProcessEvents();
        }
        
        Logger.Info("Cleaning up.");
        
        Window.Destroy();
    }

    public static void Close()
    {
        Debug.Assert(_isRunning == true);
        
        _isRunning = false;
    }
}