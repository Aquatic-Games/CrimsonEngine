using System.Runtime.CompilerServices;
using System.Text;
// ReSharper disable ExplicitCallerInfoArgument

namespace Euphoria.Core;

public static class Logger
{
    public static event OnLogMessage LogMessage;
    
    private static StringBuilder _logBuilder;

    static Logger()
    {
        LogMessage = delegate { };
        _logBuilder = new StringBuilder();
    }

    public static void Log(Severity severity, string message, [CallerLineNumber] int line = 0,
        [CallerFilePath] string file = "")
    {
        _logBuilder.Clear();

        _logBuilder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff "));

        _logBuilder.Append(severity switch
        {
            Severity.Trace => "[Trace] ",
            Severity.Debug => "[Debug] ",
            Severity.Info => "[Info]  ",
            Severity.Warning => "[Warn]  ",
            Severity.Error => "[Error] ",
            Severity.Fatal => "[Fatal] ",
            _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
        });

        string fileName = Path.GetFileName(file);
        _logBuilder.Append('(');
        _logBuilder.Append(fileName);
        _logBuilder.Append(':');
        _logBuilder.Append(line);
        _logBuilder.Append(')');
        _logBuilder.Append(' ');
        
        _logBuilder.Append(message);

        LogMessage(severity, _logBuilder.ToString());
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

    public static void UseConsole()
    {
        LogMessage += (severity, message) =>
        {
            ConsoleColor currentColor = Console.ForegroundColor;

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

            Console.ForegroundColor = currentColor;
        };

        Debug("Console logging enabled.");
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

    public delegate void OnLogMessage(Severity severity, string formattedMessage);
}