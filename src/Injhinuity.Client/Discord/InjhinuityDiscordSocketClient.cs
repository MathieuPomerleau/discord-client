using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Injhinuity.Client.Discord
{
    public interface IDiscordSocketClient
    {
        Task LoginAsync(TokenType tokenType, string token, bool validateToken = true);
        Task StartAsync();
        Task SetActivityAsync(IActivity activity);
        event Func<LogMessage, Task> Log;
        event Func<Task> LoggedIn;
        event Func<Task> Ready;
        event Func<SocketMessage, Task> MessageReceived;
    }

    public class InjhinuityDiscordSocketClient : DiscordSocketClient, IDiscordSocketClient { }
}
