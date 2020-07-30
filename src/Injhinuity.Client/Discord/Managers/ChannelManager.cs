using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Injhinuity.Client.Discord.Managers
{
    public interface IChannelManager
    {
        Task SendMessageAsync(ICommandContext context, string message);
        Task SendEmbedMessageAsync(ICommandContext context, Embed embed);
    }

    public class ChannelManager : IChannelManager
    {
        public Task SendMessageAsync(ICommandContext context, string message) =>
            context.Channel.SendMessageAsync(message);

        public Task SendEmbedMessageAsync(ICommandContext context, Embed embed) =>
            context.Channel.SendMessageAsync("", false, embed);
    }
}
