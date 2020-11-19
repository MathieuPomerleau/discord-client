using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Injhinuity.Client.Discord.Entities;

namespace Injhinuity.Client.Discord.Services
{
    public interface IInjhinuityDiscordClient
    {
        Task LoginAsync(TokenType tokenType, string token, bool validateToken = true);
        Task StartAsync();
        Task SetActivityAsync(IActivity activity);

        event Func<LogMessage, Task> Log;
        event Func<Task> LoggedIn;
        event Func<Task> Ready;
        event Func<Exception, Task> Disconnected;
        event Func<SocketMessage, Task> MessageReceived;
        event Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> ReactionAdded;
        event Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> ReactionRemoved;
        event Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, Task> ReactionsCleared;

        SocketSelfUser CurrentUser { get; }
    }

    public class InjhinuityDiscordClient : DiscordSocketClient, IInjhinuityDiscordClient
    {
        public InjhinuityDiscordClient(IInjhinuityDiscordClientConfig config) : base(config.Config) {}
    }
}
