using FluentAssertions;
using FluentAssertions.Execution;
using Injhinuity.Client.Core.Configuration.Options;
using Xunit;

namespace Injhinuity.Client.Core.Tests.Configuration.Options
{
    public class ApiOptionsTests
    {
        private readonly ApiOptions _subject;

        public ApiOptionsTests()
        {
            _subject = new ApiOptions { BaseUrl = "url" };
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
            _subject.BaseUrl = null;

            _subject.ContainsNull(result);

            using var scope = new AssertionScope();
            result.IsValid.Should().BeFalse();
            result.NullValues.Should().Contain(("Api", "BaseUrl"));
        }
    }
}
