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
        public void ContainsNull_WhenCalledWithNonNullOptions_ThenResultIsValid()
        {
            var result = new NullableOptionsResult();

            _subject.ContainsNull(result);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ContainsNull_WhenCalledWithAtLeastOneNullOptions_ThenResultIsNotValid()
        {
            var result = new NullableOptionsResult();
            _subject.LogLevel = null;

            _subject.ContainsNull(result);

            result.IsValid.Should().BeFalse();
            result.NullValues.Should().Contain(("Logging", "LogLevel"));
        }
    }
}
