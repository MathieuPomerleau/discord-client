using FluentAssertions;
using Injhinuity.Client.Core.Configuration.Options;
using Xunit;

namespace Injhinuity.Client.Core.Tests.Configuration.Options
{
    public class NullableOptionsResultTests
    {
        private readonly NullableOptionsResult _subject;

        public NullableOptionsResultTests()
        {
            _subject = new NullableOptionsResult();
        }

        [Fact]
        public void IsValid_WhenResultHasNoNullValues_ThenReturnsTrue()
        {
            var result = _subject.IsValid;

            result.Should().BeTrue();
        }

        [Fact]
        public void IsValid_WhenResultHasNullValues_ThenReturnsFalse()
        {
            _subject.AddValueToResult("name", "value");

            var result = _subject.IsValid;

            result.Should().BeFalse();
        }

        [Fact]
        public void ToString_WhenResultHasNoNullValues_ThenReturnsEmptyString()
        {
            var result = _subject.ToString();

            result.Should().BeEmpty();
        }

        [Fact]
        public void ToString_WhenResultHasNullValues_ThenReturnsItsValuesAsAString()
        {
            _subject.AddValueToResult("name", "value");

            var result = _subject.ToString();

            result.Should().Be(" name - value\n");
        }
    }
}
