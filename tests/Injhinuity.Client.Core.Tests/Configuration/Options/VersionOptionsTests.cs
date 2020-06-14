using FluentAssertions;
using Injhinuity.Client.Core.Configuration.Options;
using Xunit;

namespace Injhinuity.Client.Core.Tests.Configuration.Options
{
    public class VersionOptionsTests
    {
        private readonly VersionOptions _subject;

        public VersionOptionsTests()
        {
            _subject = new VersionOptions { VersionNo = "0" };
        }

        [Fact]
        public void ContainsNull_WhenCalledWithNonNullProperties_ThenReturnsFalse()
        {
            var result = _subject.ContainsNull();

            result.Should().BeFalse();
        }

        [Fact]
        public void ContainsNull_WhenCalledWithAtLeastOneNullOptions_ThenReturnsTrue()
        {
            _subject.VersionNo = null;

            var result = _subject.ContainsNull();

            result.Should().BeTrue();
        }
    }
}
