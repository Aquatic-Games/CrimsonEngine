using System.Diagnostics;

namespace Crimson.Engine;

public static class Metrics
{
    private static Stopwatch _timingWatch;
    
    private static double _fpsDelta;
    private static int _fpsCounter;
    private static int _fps;
    private static ulong _totalFrames;

    private static Dictionary<string, double> _performanceMetrics;

    public const string UpdateTimeMetric = "UpdateTime";

    public const string RenderTimeMetric = "RenderTime";
    
    /// <summary>
    /// The approximate number of frames per second (FPS). This value is updated once per second.
    /// </summary>
    /// <remarks>You should not use this value to truly gauge performance. Instead, use the <see cref="App.TimeSinceLastFrame"/>.</remarks>
    public static int FramesPerSecond => _fps;

    /// <summary>
    /// The total number of frames elapsed since the application started.
    /// </summary>
    public static ulong TotalFrames => _totalFrames;

    static Metrics()
    {
        _timingWatch = new Stopwatch();
        _performanceMetrics = [];
    }

    public static double GetPerformanceMetric(string name)
        => _performanceMetrics.GetValueOrDefault(name, 0);
    
    internal static void Update(double deltaTime)
    {
        _fpsDelta += deltaTime;
        if (_fpsDelta >= 1.0)
        {
            _fpsDelta -= 1.0;
            _fps = _fpsCounter;
            _fpsCounter = 0;
        }
        
        _fpsCounter++;
        _totalFrames++;
    }

    internal static void BeginPerformanceMetric()
    {
        Debug.Assert(!_timingWatch.IsRunning);
        _timingWatch.Restart();
    }

    internal static void EndPerformanceMetric(string name)
    {
        Debug.Assert(_timingWatch.IsRunning);
        _performanceMetrics[name] = _timingWatch.Elapsed.TotalSeconds;
        _timingWatch.Stop();
    }
}