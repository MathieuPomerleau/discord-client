using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using FluentAssertions;
using Injhinuity.Client.Discord.Embeds;
using Injhinuity.Client.Discord.Emotes;
using Injhinuity.Client.Discord.Entities;
using Injhinuity.Client.Discord.Services;
using NSubstitute;
using Xunit;

namespace Injhinuity.Client.Tests.Discord.Embeds
{
    public class ReactionEmbedTests
    {
        private readonly ReactionEmbed _subject;

        private readonly long _lifetimeInSeconds = 10;
        private readonly IEnumerable<ReactionButton> _buttons = new[] { new ReactionButton(InjhinuityEmote.LeftArrow, null) };
        private readonly EmbedBuilder _embedBuilder = new EmbedBuilder();

        private readonly IInjhinuityDiscordClient _discordClient;
        private readonly IReactionEmbedContent _content;
        private readonly IInjhinuityCommandContext _context;
        private readonly IMessageChannel _channel;
        private readonly IUserMessage _message;

        public ReactionEmbedTests()
        {
            _discordClient = Substitute.For<IInjhinuityDiscordClient>();
            _content = Substitute.For<IReactionEmbedContent>();
            _context = Substitute.For<IInjhinuityCommandContext>();
            _channel = Substitute.For<IMessageChannel>();
            _message = Substitute.For<IUserMessage>();

            _content.Get().Returns(_embedBuilder);
            _context.Channel.Returns(_channel);
            _channel.SendMessageAsync(default, default, default).ReturnsForAnyArgs(_message);

            _subject = new ReactionEmbed(_lifetimeInSeconds, _discordClient, _buttons, _content);
        }

        [Fact]
        public void ReactionEmbed_WhenConstructorIsCalled_ThenAssignsProperties()
        {
            _subject.Buttons.Should().BeEquivalentTo(_buttons);
            _subject.Content.Should().Be(_content);
        }

        [Fact]
        public async Task InitializeAsync_WhenConstructorIsCalled_ThenAssignsProperties()
        {
            await _subject.InitializeAsync(_context);

            _content.Received().Get();
            await _message.Received().AddReactionAsync(Arg.Any<IEmote>());
        }
    }
}
