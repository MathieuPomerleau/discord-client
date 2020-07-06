using AutoFixture;
using Discord;
using Discord.Commands;
using FluentAssertions;
using FluentAssertions.Execution;
using Injhinuity.Client.Discord.Builder;
using Xunit;

namespace Injhinuity.Client.Tests.Discord.Builder
{
    public class CommandResultBuilderTests
    {
        private static readonly IFixture _fixture = new Fixture();
        private readonly ICommandResultBuilder _subject;

        public CommandResultBuilderTests()
        {
            _subject = new CommandResultBuilder();
        }

        [Fact]
        public void Build_WhenCalledWithValues_ThenBuildsItsEmbedProperly()
        {
            var (embed, error, message, reason) = CreateValues();

            var result = _subject.WithEmbed(embed)
                .WithError(error)
                .WithMessage(message)
                .WithReason(reason)
                .Build();

            using var scope = new AssertionScope();

            result.Embed.Should().Be(embed);
            result.Error.Should().Be(error);
            result.Message.Should().Be(message);
            result.Reason.Should().Be(reason);
        }

        private (Embed, CommandError, string, string) CreateValues() =>
            (new EmbedBuilder().Build(), _fixture.Create<CommandError>(),
             _fixture.Create<string>(), _fixture.Create<string>());
    }
}
