using System.Collections;
using System.Collections.Generic;
using AutoFixture;
using Discord;
using Discord.Commands;
using FluentAssertions;
using FluentAssertions.Execution;
using Injhinuity.Client.Discord.Builders;
using Xunit;

namespace Injhinuity.Client.Tests.Discord.Results
{
    public class CommandResultBuilderTests
    {
        private static readonly IFixture _fixture = new Fixture();
        private readonly ICommandResultBuilder _subject;

        private static readonly Embed _embed = new EmbedBuilder().Build();
        private static readonly string _message = _fixture.Create<string>();
        private static readonly string _reason = _fixture.Create<string>();

        public CommandResultBuilderTests()
        {
            _subject = new CommandResultBuilder();
        }

        [Theory]
        [ClassData(typeof(TestData))]
        public void Build_WhenCalledWithValues_ThenBuildsItsEmbedProperly(CommandError error, Embed embed, string message, string reason)
        {
            var result = _subject.WithError(error)
                .WithEmbed(embed)
                .WithMessage(message)
                .WithReason(reason)
                .Build();

            using var scope = new AssertionScope();

            result.Error.Should().Be(error);
            result.Embed.Should().Be(embed);
            result.Message.Should().Be(message);
            result.Reason.Should().Be(reason);
        }

        private class TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { CommandError.Exception, _embed, _message, _reason };
                yield return new object[] { null, _embed, _message, _reason };
                yield return new object[] { CommandError.Exception, null, _message, _reason };
                yield return new object[] { CommandError.Exception, _embed, null, _reason };
                yield return new object[] { CommandError.Exception, _embed, _message, null };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
