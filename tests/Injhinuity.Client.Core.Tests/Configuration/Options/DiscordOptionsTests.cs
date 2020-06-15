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
        public void ContainsNull_WhenCalledWithNonNullOptions_ThenReturnsFalse()
        {
            var result = _subject.ContainsNull();

            result.Should().BeFalse();
        }

        [Fact]
        public void ContainsNull_WhenCalledWithAtLeastOneNullOptions_ThenReturnsTrue()
        {
            _subject.Token = null;
            _subject.Prefix = null;

            var result = _subject.ContainsNull();

            result.Should().BeTrue();
        }
    }
}
