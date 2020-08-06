using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Injhinuity.Client.Discord.Entities
{
    public interface IInjhinuityUserMessage
    {
        string Content { get; }
        bool HasCharPrefix(char prefix, ref int argPos);
        IUserMessage GetMessage();
        SocketUserMessage GetSocketMessage();
    }

    public class InjhinuityUserMessage : IInjhinuityUserMessage
    {
        private readonly SocketUserMessage _socketUserMessage;

        public InjhinuityUserMessage(SocketUserMessage socketUserMessage)
        {
            _socketUserMessage = socketUserMessage;
        }

        public string Content => _socketUserMessage.Content;

        public bool HasCharPrefix(char prefix, ref int argPos) =>
            _socketUserMessage.HasCharPrefix(prefix, ref argPos);

        public IUserMessage GetMessage() => _socketUserMessage;

        public SocketUserMessage GetSocketMessage() => _socketUserMessage;
    }
}
