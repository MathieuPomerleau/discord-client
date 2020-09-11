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
        private readonly ICommandContext _context;

        public IGuild Guild => _context.Guild;
        public IMessageChannel Channel => _context.Channel;
        public IUserMessage Message => _context.Message;
        public IGuildUser GuildUser => (IGuildUser)_context.User;

        public InjhinuityCommandContext(ICommandContext context)
        {
            _context = context;
        }

        public InjhinuityCommandContext(IInjhinuityDiscordClient client, IInjhinuityUserMessage message)
        {
            _context = new SocketCommandContext((DiscordSocketClient)client, message.GetSocketMessage());
        }

        public SocketCommandContext GetSocketContext() => (SocketCommandContext)_context;
    }
}
