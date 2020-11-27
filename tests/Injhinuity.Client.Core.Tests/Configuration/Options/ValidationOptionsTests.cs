using FluentAssertions;
using FluentAssertions.Execution;
using Injhinuity.Client.Core.Configuration.Options;
using Xunit;

namespace Injhinuity.Client.Core.Tests.Configuration.Options
{
    public class ValidationOptionsTests
    {
        private readonly ValidationOptions _subject;

        public ValidationOptionsTests()
        {
            _subject = new ValidationOptions { Command = new CommandValidationOptions { CommandNameMaxLength = 0, CommandBodyMaxLength = 0 } };
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
            _subject.Command = null;

            _subject.ContainsNull(result);

            using var scope = new AssertionScope();
            result.IsValid.Should().BeFalse();
            result.NullValues.Should().Contain(("Validation", "CommandValidation"));
        }
    }
}
