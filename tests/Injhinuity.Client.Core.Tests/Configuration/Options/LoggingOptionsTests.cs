using FluentAssertions;
using Injhinuity.Client.Core.Configuration.Options;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Injhinuity.Client.Core.Tests.Configuration.Options
{
    public class LoggingOptionsTests
    {
        private readonly LoggingOptions _subject;

        public LoggingOptionsTests()
        {
            _subject = new LoggingOptions { LogLevel = LogLevel.Information };
        }

        [Fact]
        public void ContainsNull_WhenCalledWithNonNullOptions_ThenReturnsFalse()
        {
            var result = _subject.ContainsNull();

            result.Should().BeFalse();
        }

        [Fact]
        public void ContainsNull_WhenCalledWithAtLeastOneNullOptions_ThenReturnsTrue()
        {
            _subject.LogLevel = null;

            var result = _subject.ContainsNull();

            result.Should().BeTrue();
        }
    }
}
