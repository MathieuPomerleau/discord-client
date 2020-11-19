using System.Collections;
using System.Collections.Generic;
using Discord;
using FluentAssertions;
using Injhinuity.Client.Discord.Converters;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Injhinuity.Client.Tests.Discord.Converters
{
    public class LogSeverityConverterTests
    {
        private readonly LogSeverityConverter _subject;

        public LogSeverityConverterTests()
        {
            _subject = new LogSeverityConverter();
        }

        [Theory]
        [ClassData(typeof(LogLevelTestData))]
        public void FromLogLevel_ThenReturnsConvertedResult(LogLevel logLevel, LogSeverity logSeverity)
        {
            var result = _subject.FromLogLevel(logLevel);

            result.Should().Be(logSeverity);
        }

        private class LogLevelTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { LogLevel.Debug, LogSeverity.Debug };
                yield return new object[] { LogLevel.Error, LogSeverity.Error };
                yield return new object[] { LogLevel.Critical, LogSeverity.Critical };
                yield return new object[] { LogLevel.Information, LogSeverity.Info };
                yield return new object[] { LogLevel.Trace, LogSeverity.Info };
                yield return new object[] { LogLevel.None, LogSeverity.Info };
                yield return new object[] { LogLevel.Warning, LogSeverity.Info };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
