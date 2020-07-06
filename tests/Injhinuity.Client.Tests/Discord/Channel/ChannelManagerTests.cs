using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Injhinuity.Client.Discord.Channel;
using NSubstitute;
using Xunit;

namespace Injhinuity.Client.Tests.Discord.Channel
{
    public class ChannelManagerTests
    {
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
            var message = "message";

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
