using FluentAssertions;
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
            _subject.Command = null;

            _subject.ContainsNull(result);

            result.IsValid.Should().BeFalse();
            result.NullValues.Should().Contain(("Validation", "CommandValidation"));
        }
    }
}
