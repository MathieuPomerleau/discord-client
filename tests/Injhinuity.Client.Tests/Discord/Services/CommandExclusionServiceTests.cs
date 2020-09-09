using FluentAssertions;
using Injhinuity.Client.Discord.Services;
using Xunit;

namespace Injhinuity.Client.Tests.Discord.Services
{
    public class CommandExclusionServiceTests
    {
        private readonly ICommandExclusionService _subject;

        public CommandExclusionServiceTests()
        {
            _subject = new CommandExclusionService();
        }

        [Fact]
        public void IsExcluded_WithAnExcludedCommand_ThenReturnsTrue()
        {
            var command = "info";

            var result = _subject.IsExcluded(command);

            result.Should().BeTrue();
        }

        [Fact]
        public void IsExcluded_WithAnNonExcludedCommand_ThenReturnsFalse()
        {
            var command = "command";

            var result = _subject.IsExcluded(command);

            result.Should().BeFalse();
        }
    }
}
