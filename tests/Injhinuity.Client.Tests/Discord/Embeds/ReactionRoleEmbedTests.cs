using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Discord;
using FluentAssertions;
using FluentAssertions.Execution;
using Injhinuity.Client.Discord.Embeds;
using Injhinuity.Client.Discord.Emotes;
using Injhinuity.Client.Discord.Entities;
using Injhinuity.Client.Discord.Services;
using NSubstitute;
using Xunit;

namespace Injhinuity.Client.Tests.Discord.Embeds
{
    public class ReactionRoleEmbedTests
    {
        private static readonly IFixture Fixture = new Fixture();

        private readonly ReactionRoleEmbed _subject;

        private readonly ulong _messageId = Fixture.Create<ulong>();
        private readonly IEnumerable<UserReactionButton> _buttons = new[] { new UserReactionButton(InjhinuityEmote.LeftArrow, default) };
        private readonly EmbedBuilder _embedBuilder = new EmbedBuilder();

        private readonly IInjhinuityDiscordClient _discordClient;
        private readonly IEmbedContent _content;
        private readonly IInjhinuityCommandContext _context;
        private readonly IMessageChannel _channel;
        private readonly IUserMessage _message;

        public ReactionRoleEmbedTests()
        {
            _discordClient = Substitute.For<IInjhinuityDiscordClient>();
            _content = Substitute.For<IEmbedContent>();
            _context = Substitute.For<IInjhinuityCommandContext>();
            _channel = Substitute.For<IMessageChannel>();
            _message = Substitute.For<IUserMessage>();

            _content.Get().Returns(_embedBuilder);
            _context.Channel.Returns(_channel);
            _channel.SendMessageAsync(default, default, default).ReturnsForAnyArgs(_message);
            _message.Id.Returns(_messageId);

            _subject = new ReactionRoleEmbed(_discordClient, _buttons, _content);
        }

        [Fact]
        public void ReactionEmbed_ThenAssignsProperties()
        {
            using var scope = new AssertionScope();
            _subject.Buttons.Should().BeEquivalentTo(_buttons);
            _subject.Content.Should().Be(_content);
        }

        [Fact]
        public async Task InitalizeAsync_WithContext_ThenInitializesTheEmbed()
        {
            var result = await _subject.InitalizeAsync(_context);

            result.Should().Be(_messageId);
            _content.Received().Get();
            await _channel.Received().SendMessageAsync("", false, Arg.Any<Embed>());
            await _message.Received().AddReactionAsync(Arg.Any<IEmote>());
        }

        [Fact]
        public async Task InitalizeFromExistingAsync_WithMessage_ThenInitializesTheEmbed()
        {
            await _subject.InitalizeFromExistingAsync(_message);

            _content.Received().Get();
            await _message.Received().ModifyAsync(Arg.Any<Action<MessageProperties>>());
            await _message.Received().RemoveAllReactionsAsync();
            await _message.Received().AddReactionAsync(Arg.Any<IEmote>());
        }

        [Fact]
        public async Task ResetAsync_WithNullMessage_ThenDoesntDeleteTheMessage()
        {
            await _subject.ResetAsync();

            await _message.DidNotReceive().DeleteAsync();
        }

        [Fact]
        public async Task ResetAsync_WithMessage_ThenDeletesTheMessage()
        {
            await _subject.InitalizeFromExistingAsync(_message);
            await _subject.ResetAsync();

            await _message.Received().DeleteAsync();
        }
    }
}
