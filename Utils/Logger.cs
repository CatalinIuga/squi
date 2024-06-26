namespace squi.Utils;

public class Logger
{
    private enum LogLevel
    {
        DEBUG,
        INF0,
        WARN,
        ERROR
    }

    private static string StyleMap(LogLevel? level) =>
        level switch
        {
            LogLevel.DEBUG => "\u001b[1;35m",
            LogLevel.INF0 => "\u001b[1;34m",
            LogLevel.WARN => "\u001b[1;33m",
            LogLevel.ERROR => "\u001b[1;31m",
            // reset color
            _ => "\u001b[0m"
        };

    private static string GetTime()
    {
        return DateTime.Now.ToString("HH:mm");
    }

    private static void LogWithLevel(LogLevel level, string message)
    {
        switch (level)
        {
            case LogLevel.DEBUG:
                Console.Write($"{GetTime()} ");
                Console.Write(StyleMap(level));
                Console.Write(nameof(LogLevel.DEBUG)[..4]);
                Console.Write(StyleMap(null));
                Console.WriteLine($" {message}");

                break;
            case LogLevel.INF0:
                Console.Write($"{GetTime()} ");
                Console.Write(StyleMap(level));
                Console.Write(nameof(LogLevel.INF0)[..4]);
                Console.Write(StyleMap(null));
                Console.WriteLine($" {message}");
                break;
            case LogLevel.WARN:
                Console.Write($"{GetTime()} ");
                Console.Write(StyleMap(level));
                Console.Write(nameof(LogLevel.WARN)[..4]);
                Console.Write(StyleMap(null));
                Console.WriteLine($" {message}");
                break;
            case LogLevel.ERROR:
                Console.Write($"{GetTime()} ");
                Console.Write(StyleMap(level));
                Console.Write(nameof(LogLevel.ERROR)[..4]);
                Console.Write(StyleMap(null));
                Console.WriteLine($" {message}");
                break;
        }
    }

    public static void Debug(string message)
    {
        LogWithLevel(LogLevel.DEBUG, message);
    }

    public static void Info(string message)
    {
        LogWithLevel(LogLevel.INF0, message);
    }

    public static void Warn(string message)
    {
        LogWithLevel(LogLevel.WARN, message);
    }

    public static void Error(string message)
    {
        LogWithLevel(LogLevel.ERROR, message);
    }
}
