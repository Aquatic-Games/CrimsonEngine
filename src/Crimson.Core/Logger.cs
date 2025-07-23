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

    private static bool _logToConsole;
    private static string? _logFile;
    private static StreamWriter? _writer;

    /// <summary>
    /// If true, the logger will output <see cref="Trace"/> logs to the log file. In release builds, this defaults to
    /// false as the trace logs clog the output and generally aren't necessary, however may be useful for debugging.
    /// </summary>
    /// <remarks>Trace logs are <b>always</b> output to the console if <see cref="LogToConsole"/> is enabled.</remarks>
    public static bool OutputTraceLogs;

    /// <summary>
    /// Enable or disable console logging. Debug messages will be output to the console.
    /// </summary>
    public static bool LogToConsole
    {
        get => _logToConsole;
        set
        {
            
            
            // Don't attach the console if it's already attached! This will result in duplicate outputs and also a
            // memory leak.
            if (value && _logToConsole)
                return;
            _logToConsole = value;

            if (value)
                LogMessage += WriteMessageToConsole;
            else
                LogMessage -= WriteMessageToConsole;
        }
    }

    /// <summary>
    /// Get or set the current log file path. If this value is null, logging to a file is disabled. 
    /// </summary>
    public static string? LogFile
    {
        get => _logFile;
        set
        {
            _writer?.Close();
            _logFile = value;

            if (value == null)
            {
                LogMessage -= WriteMessageToFile;
                return;
            }
            
            Directory.CreateDirectory(Path.GetDirectoryName(value));

            _writer = new StreamWriter(value)
            {
                AutoFlush = true
            };

            LogMessage += WriteMessageToFile;
        }
    }

    static Logger()
    {
        LogMessage = delegate { };
        _logBuilder = new StringBuilder();
#if DEBUG
        OutputTraceLogs = true;
#endif
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
    
    private static void WriteMessageToConsole(Severity severity, string formattedMessage)
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

        Console.WriteLine(formattedMessage);

        Console.ForegroundColor = currentColor;
    }

    private static void WriteMessageToFile(Severity severity, string formattedMessage)
    {
        if (severity == Severity.Trace && !OutputTraceLogs)
            return;
        
        _writer!.WriteLine(formattedMessage);
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