using Microsoft.Extensions.Logging;

namespace Catalog.IntegrationTests.TestUtils;

public class NullLogger<T>: ILogger<T>
{
    public void Log(string message)
    {
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        throw new NotImplementedException();
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        throw new NotImplementedException();
    }
}