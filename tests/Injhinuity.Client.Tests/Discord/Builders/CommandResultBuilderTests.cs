using System.Collections;
using System.Collections.Generic;
using AutoFixture;
using Discord;
using Discord.Commands;
using FluentAssertions;
using FluentAssertions.Execution;
using Injhinuity.Client.Discord.Builders;
using Injhinuity.Client.Discord.Embeds;
using NSubstitute;
using Xunit;

namespace Injhinuity.Client.Tests.Discord.Results
{
    public class CommandResultBuilderTests
    {
        private static readonly IFixture Fixture = new Fixture();
        private readonly ICommandResultBuilder _subject;

        private static readonly IReactionEmbed _reactionEmbed = Substitute.For<IReactionEmbed>();
        private static readonly EmbedBuilder _embedBuilder = new EmbedBuilder();
        private static readonly string _message = Fixture.Create<string>();
        private static readonly string _reason = Fixture.Create<string>();

        public CommandResultBuilderTests()
        {
            _subject = new CommandResultBuilder();
        }

        [Theory]
        [ClassData(typeof(TestData))]
        public void Build_WhenCalledWithValues_ThenBuildsItsEmbedProperly(CommandError error, EmbedBuilder embedBuilder,
            IReactionEmbed reactionEmbed, string message, string reason)
        {
            var result = _subject.Create()
                .WithError(error)
                .WithEmbedBuilder(embedBuilder)
                .WithReactionEmbed(reactionEmbed)
                .WithMessage(message)
                .WithReason(reason)
                .Build();

            using var scope = new AssertionScope();

            result.Error.Should().Be(error);
            result.EmbedBuilder.Should().Be(embedBuilder);
            result.ReactionEmbed.Should().Be(reactionEmbed);
            result.Message.Should().Be(message);
            result.Reason.Should().Be(reason);
        }

        private class TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { CommandError.Exception, _embedBuilder, _reactionEmbed, _message, _reason };
                yield return new object[] { null, _embedBuilder, _reactionEmbed, _message, _reason };
                yield return new object[] { CommandError.Exception, null, _reactionEmbed, _message, _reason };
                yield return new object[] { CommandError.Exception, _embedBuilder, null, _message, _reason };
                yield return new object[] { CommandError.Exception, _embedBuilder, _reactionEmbed, null, _reason };
                yield return new object[] { CommandError.Exception, _embedBuilder, _reactionEmbed, _message, null };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
