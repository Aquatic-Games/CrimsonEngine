using System.Diagnostics;
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
        
        _appName = options.Name;
        
        Window.Create(in options.Window);
        Events.WindowClose += Close;
        
        _isRunning = true;

        while (_isRunning)
        {
            Events.ProcessEvents();
        }
        
        Window.Destroy();
    }

    public static void Close()
    {
        Debug.Assert(_isRunning == true);
        
        _isRunning = false;
    }
}