using System.Diagnostics;

namespace Crimson.Core;

public static class Metrics
{
    public const string UpdateTimeMetric = "UpdateTime";

    public const string RenderTimeMetric = "RenderTime";
    
    private static double _fpsDelta;
    private static int _fpsCounter;
    private static int _fps;
    private static ulong _totalFrames;

    private static Dictionary<string, Stopwatch> _performanceMetrics;
    
    /// <summary>
    /// The approximate number of frames per second (FPS). This value is updated once per second.
    /// </summary>
    /// <remarks>You should not use this value to truly gauge performance. Instead, use the <see cref="Crimson.Engine.App.TimeSinceLastFrame"/>.</remarks>
    public static int FramesPerSecond => _fps;

    /// <summary>
    /// The total number of frames elapsed since the application started.
    /// </summary>
    public static ulong TotalFrames => _totalFrames;

    static Metrics()
    {
        _performanceMetrics = [];
    }
    
    public static void BeginPerformanceMetric(string name)
    {
        if (!_performanceMetrics.TryGetValue(name, out Stopwatch sw))
        {
            sw = new Stopwatch();
            _performanceMetrics.Add(name, sw);
        }
        
        sw.Restart();
    }

    public static void EndPerformanceMetric(string name)
    {
        _performanceMetrics[name].Stop();
    }

    public static TimeSpan GetPerformanceMetric(string name)
    {
        if (!_performanceMetrics.TryGetValue(name, out Stopwatch sw))
            return TimeSpan.Zero;

        return sw.Elapsed;
    }
    
    public static void Update(double deltaTime)
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
}