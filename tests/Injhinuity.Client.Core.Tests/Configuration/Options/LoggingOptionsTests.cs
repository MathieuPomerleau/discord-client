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
            _subject = new LoggingOptions
            {
                AppLogLevel = LogLevel.Information,
                DiscordLogLevel = LogLevel.Information
            };
        }

        [Fact]
        public void ContainsNull_WithNonNullOptions_ThenResultIsValid()
        {
            var result = new NullableOptionsResult();

            _subject.ContainsNull(result);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ContainsNull_WithAtLeastOneNullOptions_ThenResultIsNotValid()
        {
            var result = new NullableOptionsResult();
            _subject.AppLogLevel = null;

            _subject.ContainsNull(result);

            result.IsValid.Should().BeFalse();
            result.NullValues.Should().Contain(("Logging", "AppLogLevel"));
        }
    }
}
