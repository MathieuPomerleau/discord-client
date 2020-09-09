using FluentAssertions;
using Injhinuity.Client.Core.Configuration.Options;
using Xunit;

namespace Injhinuity.Client.Core.Tests.Configuration.Options
{
    public class DiscordOptionsTests
    {
        private readonly DiscordOptions _subject;

        public DiscordOptionsTests()
        {
            _subject = new DiscordOptions { Token = "token", Prefix = '!' };
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
            _subject.Token = null;
            _subject.Prefix = null;

            _subject.ContainsNull(result);

            result.IsValid.Should().BeFalse();
            result.NullValues.Should().Contain(("Discord", "Token"));
            result.NullValues.Should().Contain(("Discord", "Prefix"));
        }
    }
}
