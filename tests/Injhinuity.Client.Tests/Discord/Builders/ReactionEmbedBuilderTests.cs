using System;
using AutoFixture;
using Discord;
using FluentAssertions;
using Injhinuity.Client.Core.Exceptions;
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
        private static readonly long _lifetimeInSeconds = Fixture.Create<long>();

        private readonly IEmbedContent _content;
        private readonly IInjhinuityDiscordClient _discordClient;

        public ReactionEmbedBuilderTests()
        {
            _content = Substitute.For<IEmbedContent>();
            _discordClient = Substitute.For<IInjhinuityDiscordClient>();

            _subject = new ReactionEmbedBuilder(_discordClient);
        }

        [Fact]
        public void BuildPage_WithValuesAndSingleButton_ThenProperlyBuildsEmbedBuilder()
        {
            var result = _subject.Create()
                .WithButton(_emote, default)
                .WithContent(_content)
                .WithLifetime(_lifetimeInSeconds)
                .BuildPage();

            result.Should().BeOfType<PageReactionEmbed>();
        }

        [Fact]
        public void BuildPage_WithValuesAndMultipleButtons_ThenProperlyBuildsEmbedBuilder()
        {
            var result = _subject.Create()
                .WithButtons(new[] { new ReactionButton(_emote, default), new ReactionButton(_emote, default) })
                .WithContent(_content)
                .WithLifetime(_lifetimeInSeconds)
                .BuildPage();

            result.Should().BeOfType<PageReactionEmbed>();
        }

        [Fact]
        public void BuildPage_WithoutContentAndButtons_ThenThrowsException()
        {
            Action result = () => _subject.Create().BuildPage();

            result.Should().Throw<InjhinuityException>().WithMessage("No content or buttons provided for PageReactionEmbed.");
        }

        [Fact]
        public void BuildPage_WithoutContent_ThenThrowsException()
        {
            Action result = () => _subject.Create().WithButton(_emote, default).BuildPage();

            result.Should().Throw<InjhinuityException>().WithMessage("No content or buttons provided for PageReactionEmbed.");
        }

        [Fact]
        public void BuildPage_WithoutButtons_ThenThrowsException()
        {
            Action result = () => _subject.Create().WithContent(_content).BuildPage();

            result.Should().Throw<InjhinuityException>().WithMessage("No content or buttons provided for PageReactionEmbed.");
        }

        [Fact]
        public void BuildRole_WithValuesAndMultipleButtons_ThenProperlyBuildsEmbedBuilder()
        {
            var result = _subject.Create()
                .WithUserButtons(new[] { new UserReactionButton(_emote, default), new UserReactionButton(_emote, default) })
                .WithContent(_content)
                .BuildRole();

            result.Should().BeOfType<ReactionRoleEmbed>();
        }

        [Fact]
        public void BuildRole_WithoutContent_ThenThrowsException()
        {
            Action result = () => _subject.Create().BuildRole();

            result.Should().Throw<InjhinuityException>().WithMessage("No content provided for RoleReactionEmbed.");
        }
    }
}
