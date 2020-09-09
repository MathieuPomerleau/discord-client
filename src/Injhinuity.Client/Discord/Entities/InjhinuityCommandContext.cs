using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Injhinuity.Client.Discord.Services;

namespace Injhinuity.Client.Discord.Entities
{
    public interface IInjhinuityCommandContext
    {
        IGuild Guild { get; }
        IMessageChannel Channel { get; }
        IUserMessage Message { get; }
        IGuildUser GuildUser { get; }
        SocketCommandContext GetSocketContext();
    }

    public class InjhinuityCommandContext : IInjhinuityCommandContext
    {
        private readonly ICommandContext Context;

        public IGuild Guild => Context.Guild;
        public IMessageChannel Channel => Context.Channel;
        public IUserMessage Message => Context.Message;
        public IGuildUser GuildUser => (IGuildUser)Context.User;

        public InjhinuityCommandContext(ICommandContext context)
        {
            Context = context;
        }

        public InjhinuityCommandContext(IInjhinuityDiscordClient client, IInjhinuityUserMessage message)
        {
            Context = new SocketCommandContext((DiscordSocketClient)client, message.GetSocketMessage());
        }

        public SocketCommandContext GetSocketContext() => (SocketCommandContext)Context;
    }
}
