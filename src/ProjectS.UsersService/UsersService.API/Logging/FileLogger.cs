namespace UsersService.API.Logging;

public class FileLogger(string categoryName, string logsDirectory) : ILogger
{
    private readonly string _categoryName = categoryName;
    private readonly string _logsDirectory = logsDirectory;
    private static readonly object _lock = new();

    IDisposable ILogger.BeginScope<TState>(TState state) => null!;

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        // Determine which file to write to based on the log level
        string filePath = logLevel >= LogLevel.Error
            ? GetDailyLogFilePath("exceptions")
            : GetDailyLogFilePath("application");

        // Prepare the log message
        string logMessage = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} [{logLevel}] {_categoryName}: {formatter(state, exception)}";

        if (exception != null)
        {
            logMessage += $"{Environment.NewLine}Exception Type: {exception.GetType().Name}{Environment.NewLine}" +
                          $"Message: {exception.Message}{Environment.NewLine}" +
                          $"StackTrace: {exception.StackTrace}{Environment.NewLine}";
        }

        // Write log message to the appropriate log file
        WriteLog(filePath, logMessage);
    }

    /// <summary>
    /// Generates a file path for daily logs based on the log type.
    /// The file name includes the log type and the current date.
    /// </summary>
    /// <param name="logType">Type of log (e.g., "application", "exceptions").</param>
    /// <returns>Full path to the log file.</returns>
    private string GetDailyLogFilePath(string logType)
    {
        // Generate a daily log file name
        string date = DateTime.UtcNow.ToString("yyyy-MM-dd");
        string fileName = $"{logType}-{date}.log";

        // Combine with the logs directory
        return Path.Combine(_logsDirectory, fileName);
    }

    /// <summary>
    /// Writes a log message to the specified file.
    /// Ensures thread safety by using a lock.
    /// </summary>
    /// <param name="filePath">Path to the log file.</param>
    /// <param name="message">Log message to write.</param>
    private static void WriteLog(string filePath, string message)
    {
        lock (_lock)
        {
            File.AppendAllText(filePath, message + Environment.NewLine);
        }
    }
}
