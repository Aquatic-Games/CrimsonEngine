using System.Runtime.CompilerServices;
using System.Text;
// ReSharper disable ExplicitCallerInfoArgument

namespace Euphoria.Core;

public static class Logger
{
    public static event OnLogMessage LogMessage;
    
    private static StringBuilder _builder;

    private static StreamWriter? _writer;

    static Logger()
    {
        LogMessage = delegate { };
        _builder = new StringBuilder();
        _writer = null;
    }
    
    public static void Log(Severity severity, string message, [CallerLineNumber] int line = 0, [CallerFilePath] string file = "")
    {
        _builder.Clear();

        _builder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff "));
        
        _builder.Append(severity switch
        {
            Severity.Trace   => "[Trace] ",
            Severity.Debug   => "[Debug] ",
            Severity.Info    => "[Info]  ",
            Severity.Warning => "[Warn]  ",
            Severity.Error   => "[Error] ",
            Severity.Fatal   => "[Fatal] ",
            _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
        });

        _builder.Append('(');
        _builder.Append(Path.GetFileName(file));
        _builder.Append(':');
        _builder.Append(line);
        _builder.Append(") ");

        _builder.Append(message);

        LogMessage(severity, _builder.ToString());
    }

    public static void Trace(string message, [CallerLineNumber] int line = 0, [CallerFilePath] string file = "")
        => Log(Severity.Trace, message, line, file);
    
    public static void Debug(string message, [CallerLineNumber] int line = 0, [CallerFilePath] string file = "")
        => Log(Severity.Debug, message, line, file);
    
    public static void Info(string message, [CallerLineNumber] int line = 0, [CallerFilePath] string file = "")
        => Log(Severity.Info, message, line, file);
    
    public static void Warn(string message, [CallerLineNumber] int line = 0, [CallerFilePath] string file = "")
        => Log(Severity.Warning, message, line, file);
    
    public static void Error(string message, [CallerLineNumber] int line = 0, [CallerFilePath] string file = "")
        => Log(Severity.Error, message, line, file);
    
    public static void Fatal(string message, [CallerLineNumber] int line = 0, [CallerFilePath] string file = "")
        => Log(Severity.Fatal, message, line, file);
    
    public static void Fatal(Exception exception, [CallerLineNumber] int line = 0, [CallerFilePath] string file = "")
        => Log(Severity.Fatal, exception.ToString(), line, file);

    public static void EnableConsole()
    {
        LogMessage += LogConsoleMessage;
    }

    public static void DisableConsole()
    {
        LogMessage -= LogConsoleMessage;
    }

    public static void EnableFile(string path, bool append = false)
    {
        Debug($"Writing log file to {path}.");
        
        _writer = new StreamWriter(path, append) { AutoFlush = true };
        LogMessage += LogFileMessage;
    }

    public static void DisableFile()
    {
        LogMessage -= LogFileMessage;
        _writer?.Dispose();
    }

    private static void LogConsoleMessage(Severity severity, string message)
    {
        ConsoleColor color = Console.ForegroundColor;

        Console.ForegroundColor = severity switch
        {
            Severity.Trace => ConsoleColor.Gray,
            Severity.Debug => ConsoleColor.White,
            Severity.Info => ConsoleColor.Cyan,
            Severity.Warning => ConsoleColor.Yellow,
            Severity.Error => ConsoleColor.Red,
            Severity.Fatal => ConsoleColor.DarkRed,
            _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
        };
        
        Console.WriteLine(message);

        Console.ForegroundColor = color;
    }

    private static void LogFileMessage(Severity severity, string message)
    {
        _writer!.WriteLine(message);
    }
    
    public enum Severity
    {
        Trace,
        Debug,
        Info,
        Warning,
        Error,
        Fatal
    }

    public delegate void OnLogMessage(Severity severity, string message);
}