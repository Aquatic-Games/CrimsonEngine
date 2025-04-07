using System.Runtime.CompilerServices;
using System.Text;
// ReSharper disable ExplicitCallerInfoArgument

namespace Crimson.Core;

/// <summary>
/// Contains logging functions which can be used to aid with diagnostics.
/// </summary>
public static class Logger
{
    /// <summary>
    /// Invoked when a log message is received.
    /// </summary>
    public static event OnLogMessage LogMessage;
    
    private static StringBuilder _logBuilder;

    static Logger()
    {
        LogMessage = delegate { };
        _logBuilder = new StringBuilder();
    }

    /// <summary>
    /// Log a message.
    /// </summary>
    /// <param name="severity">The message's <see cref="Severity"/>.</param>
    /// <param name="message">The message.</param>
    /// <param name="line">The line number where this method was called.</param>
    /// <param name="file">The file where this method was called.</param>
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

    /// <summary>
    /// Log a <see cref="Severity.Trace"/> message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="line">The line number where this method was called.</param>
    /// <param name="file">The file where this method was called.</param>
    public static void Trace(string message, [CallerLineNumber] int line = 0, [CallerFilePath] string file = "")
        => Log(Severity.Trace, message, line, file);
    
    /// <summary>
    /// Log a <see cref="Severity.Debug"/> message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="line">The line number where this method was called.</param>
    /// <param name="file">The file where this method was called.</param>
    public static void Debug(string message, [CallerLineNumber] int line = 0, [CallerFilePath] string file = "")
        => Log(Severity.Debug, message, line, file);
    
    /// <summary>
    /// Log a <see cref="Severity.Info"/> message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="line">The line number where this method was called.</param>
    /// <param name="file">The file where this method was called.</param>
    public static void Info(string message, [CallerLineNumber] int line = 0, [CallerFilePath] string file = "")
        => Log(Severity.Info, message, line, file);
    
    /// <summary>
    /// Log a <see cref="Severity.Warning"/> message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="line">The line number where this method was called.</param>
    /// <param name="file">The file where this method was called.</param>
    public static void Warn(string message, [CallerLineNumber] int line = 0, [CallerFilePath] string file = "")
        => Log(Severity.Warning, message, line, file);
    
    /// <summary>
    /// Log an <see cref="Severity.Error"/> message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="line">The line number where this method was called.</param>
    /// <param name="file">The file where this method was called.</param>
    public static void Error(string message, [CallerLineNumber] int line = 0, [CallerFilePath] string file = "")
        => Log(Severity.Error, message, line, file);
    
    /// <summary>
    /// Log a <see cref="Severity.Fatal"/> message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="line">The line number where this method was called.</param>
    /// <param name="file">The file where this method was called.</param>
    public static void Fatal(string message, [CallerLineNumber] int line = 0, [CallerFilePath] string file = "")
        => Log(Severity.Fatal, message, line, file);

    /// <summary>
    /// Enable and console logging. Debug messages will be output to the console.
    /// </summary>
    public static void EnableConsole()
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
    
    /// <summary>
    /// Defines the severity of a log message.
    /// </summary>
    public enum Severity
    {
        /// <summary>
        /// Trace/Verbose message. Used to perform verbose diagnostics.
        /// </summary>
        Trace,
        
        /// <summary>
        /// Debug message. Used to aid in debugging.
        /// </summary>
        Debug,
        
        /// <summary>
        /// Info message. Used when something of note is logged.
        /// </summary>
        Info,
        
        /// <summary>
        /// Warning message. Used when something unexpected happened, but was handled.
        /// </summary>
        Warning,
        
        /// <summary>
        /// Error message. Used when something unexpected happened, but execution can continue.
        /// </summary>
        Error,
        
        /// <summary>
        /// Fatal message. Used when something unexpected happened and execution cannot continue.
        /// </summary>
        Fatal
    }

    /// <summary>
    /// Used in the <see cref="Logger.LogMessage"/> event.
    /// </summary>
    public delegate void OnLogMessage(Severity severity, string formattedMessage);
}