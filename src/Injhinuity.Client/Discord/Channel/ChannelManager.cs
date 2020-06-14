using System.Threading.Tasks;
using Discord.Commands;

namespace Injhinuity.Client.Discord.Channel
{
    public interface IChannelManager
    {
        Task SendMessageAsync(ICommandContext context, string message);
    }

    public class ChannelManager : IChannelManager
    {
        public Task SendMessageAsync(ICommandContext context, string message) =>
            context.Channel.SendMessageAsync(message);
    }
}
