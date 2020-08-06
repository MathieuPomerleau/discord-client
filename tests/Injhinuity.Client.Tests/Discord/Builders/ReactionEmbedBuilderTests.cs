using System;
using AutoFixture;
using Discord;
using FluentAssertions;
using Injhinuity.Client.Discord.Builders;
using Injhinuity.Client.Discord.Embeds;
using Injhinuity.Client.Discord.Emotes;
using Injhinuity.Client.Discord.Services;
using NSubstitute;
using Xunit;

namespace Injhinuity.Client.Tests.Discord.Builders
{
    public class ReactionEmbedBuilderTests
    {
        private static readonly IFixture Fixture = new Fixture();
        private readonly IReactionEmbedBuilder _subject;

        private static readonly IEmote _emote = InjhinuityEmote.LeftArrow;
        private static readonly IReactionEmbedContent _content = Substitute.For<IReactionEmbedContent>();
        private static readonly long _lifetimeInSeconds = Fixture.Create<long>();

        private readonly IInjhinuityDiscordClient _discordClient;

        public ReactionEmbedBuilderTests()
        {
            _discordClient = Substitute.For<IInjhinuityDiscordClient>();

            _subject = new ReactionEmbedBuilder(_discordClient);
        }

        [Fact]
        public void Build_WhenCalledWithValues_ThenProperlyBuildsEmbedBuilder()
        {
            var result = _subject.Create()
                .WithButton(_emote, null)
                .WithContent(_content)
                .WithLifetime(_lifetimeInSeconds)
                .Build();

            result.Should().BeOfType<ReactionEmbed>();
        }

        [Fact]
        public void Build_WhenCalledWithoutContent_ThenThrowsException()
        {
            Action result = () => _subject.Create().Build();

            result.Should().Throw<ArgumentException>();
        }
    }
}
