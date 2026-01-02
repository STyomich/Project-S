namespace UsersService.API.Logging;

public class FileLoggerProvider : ILoggerProvider
{
    private readonly string _logsDirectory;

    private bool _disposed;

    public FileLoggerProvider(string logsDirectory)
    {
        _logsDirectory = logsDirectory;

        Directory.CreateDirectory(logsDirectory);
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(categoryName, _logsDirectory);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            _disposed = true;
        }
    }
}