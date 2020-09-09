using FluentAssertions;
using Injhinuity.Client.Core.Configuration.Options;
using Xunit;

namespace Injhinuity.Client.Core.Tests.Configuration.Options
{
    public class CommandValidationOptionsTests
    {
        private readonly CommandValidationOptions _subject;

        public CommandValidationOptionsTests()
        {
            _subject = new CommandValidationOptions { CommandNameMaxLength = 0, CommandBodyMaxLength = 0 };
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
            _subject.CommandNameMaxLength = null;

            _subject.ContainsNull(result);

            result.IsValid.Should().BeFalse();
            result.NullValues.Should().Contain(("CommandValidation", "CommandNameMaxLength"));
        }
    }
}
