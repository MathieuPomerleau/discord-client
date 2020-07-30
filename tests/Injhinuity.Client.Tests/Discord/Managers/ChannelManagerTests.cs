using System.Threading.Tasks;
using AutoFixture;
using Discord;
using Discord.Commands;
using Injhinuity.Client.Discord.Managers;
using NSubstitute;
using Xunit;

namespace Injhinuity.Client.Tests.Discord.Managers
{
    public class ChannelManagerTests
    {
        private static readonly IFixture _fixture = new Fixture();
        private readonly IChannelManager _subject;

        private readonly ICommandContext _commandContext;
        private readonly IMessageChannel _messageChannel;

        public ChannelManagerTests()
        {
            _commandContext = Substitute.For<ICommandContext>();
            _messageChannel = Substitute.For<IMessageChannel>();
            _subject = new ChannelManager();

            _commandContext.Channel.Returns(_messageChannel);
        }

        [Fact]
        public async Task SendMessageAsync_WhenCalled_ThenSendsTheMessageOnTheContextChannel()
        {
            var message = _fixture.Create<string>();

            await _subject.SendMessageAsync(_commandContext, message);

            await _messageChannel.Received().SendMessageAsync(message);
        }

        [Fact]
        public async Task SendEmbedMessageAsync_WhenCalled_ThenSendsTheMessageOnTheContextChannel()
        {
            var embed = new EmbedBuilder().Build();

            await _subject.SendEmbedMessageAsync(_commandContext, embed);

            await _messageChannel.Received().SendMessageAsync("", false, embed);
        }
    }
}
