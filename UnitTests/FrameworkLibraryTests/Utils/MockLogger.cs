using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace UnitTests.FrameworkLibraryTests.Utils
{
    public class MockLogger<T> : ILogger<T>
    {
        public class MockScope : IDisposable
        {
            public void Dispose()
            {

            }
        }

        readonly Stack<ReceivedLogEventInfo> _receivedLogEvents = new Stack<ReceivedLogEventInfo>();

        public IDisposable BeginScope<TState>(TState state) => new MockScope();

        public bool IsEnabled(LogLevel logLevel) => true;

        public int CurrentLogEventCount => _receivedLogEvents.Count;

        public IEnumerable<ReceivedLogEventInfo> ReceivedLogEvents => _receivedLogEvents.ToList();

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            _receivedLogEvents.Push(new ReceivedLogEventInfo { LogLevel = logLevel, Message = state?.ToString() });
        }

        public void ReceivedLogError() => ReceivedAny(LogLevel.Error);

        public void ReceivedLogError(int count) => ReceivedCount(count, LogLevel.Error);

        public void ReceivedLogWarning() => ReceivedAny(LogLevel.Warning);

        public void ReceivedLogWarning(int count) => ReceivedCount(count, LogLevel.Warning);

        public void ReceivedLogInfo() => ReceivedAny(LogLevel.Information);

        public void ReceivedLogInfo(int count) => ReceivedCount(count, LogLevel.Information);

        public void ReceivedLogDebug() => ReceivedAny(LogLevel.Debug);

        public void ReceivedLogDebug(int count) => ReceivedCount(count, LogLevel.Debug);

        public void ReceivedLogCritical() => ReceivedAny(LogLevel.Critical);

        public void ReceivedLogCritical(int count) => ReceivedCount(count, LogLevel.Critical);

        public void ReceivedLogTrace() => ReceivedAny(LogLevel.Trace);

        public void ReceivedLogTrace(int count) => ReceivedCount(count, LogLevel.Trace);

        public void ReceivedCount(int count, LogLevel logLevel)
        {
            var actualCount = _receivedLogEvents.Count(e => e.LogLevel == logLevel);

            Assert.Equal(actualCount, count);
        }

        public void ReceivedAny(LogLevel logLevel) => Assert.Contains(_receivedLogEvents, e => e.LogLevel == logLevel);

        public class ReceivedLogEventInfo
        {
            public LogLevel LogLevel { get; set; }

            public string? Message { get; set; }
        }
    }
}
