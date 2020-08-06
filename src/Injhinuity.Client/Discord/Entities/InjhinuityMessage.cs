using Discord;
using Discord.WebSocket;

namespace Injhinuity.Client.Discord.Entities
{
    public interface IInjhinuityMessage
    {
        IMessage GetMessage();
        SocketMessage GetSocketMessage();
    }

    public class InjhinuityMessage : IInjhinuityMessage
    {
        private readonly SocketMessage _socketMessage;

        public InjhinuityMessage(SocketMessage socketMessage)
        {
            _socketMessage = socketMessage;
        }

        public IMessage GetMessage() => _socketMessage;
        public SocketMessage GetSocketMessage() => _socketMessage;
    }
}
