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
        public void ContainsNull_WithNonNullProperties_ThenResultIsValid()
        {
            var result = new NullableOptionsResult();
            
            _subject.ContainsNull(result);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ContainsNull_WithAtLeastOneNullOptions_ThenResultIsNotValid()
        {
            var result = new NullableOptionsResult();
            _subject.VersionNo = null;

            _subject.ContainsNull(result);

            result.IsValid.Should().BeFalse();
            result.NullValues.Should().Contain(("Version", "VersionNo"));
        }
    }
}
