using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Injhinuity.Client.Discord.Services;

namespace Injhinuity.Client.Discord.Wrappers
{
    public interface IUserMessageReactionWrapper : IDisposable
    {
    }

    public class UserMessageReactionWrapper : IUserMessageReactionWrapper
    {
        private bool _disposing = false;
        private readonly IUserMessage _message;
        private readonly IInjhinuityDiscordClient _discordClient;

        private event Func<SocketReaction, Task>? OnReactionAdded;
        private event Func<SocketReaction, Task>? OnReactionRemoved;

        public UserMessageReactionWrapper(IUserMessage message, IInjhinuityDiscordClient discordClient, Func<SocketReaction, Task> reactionAdded, Func<SocketReaction, Task> reactionRemoved)
        {
            _message = message;
            _discordClient = discordClient;

            _discordClient.ReactionAdded += ReactionAdded;
            _discordClient.ReactionRemoved += ReactionRemoved;

            OnReactionAdded += reactionAdded;
            OnReactionRemoved += reactionRemoved;
        }

        private Task ReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel messageChannel, SocketReaction reaction)
        {
            if (_message.Id == message.Id)
                OnReactionAdded?.Invoke(reaction);

            return Task.CompletedTask;
        }

        private Task ReactionRemoved(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel messageChannel, SocketReaction reaction)
        {
            if (_message.Id == message.Id)
                OnReactionRemoved?.Invoke(reaction);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (_disposing) 
                return;
            
            _disposing = true;

            _discordClient.ReactionAdded -= ReactionAdded;
            _discordClient.ReactionRemoved -= ReactionRemoved;

            OnReactionAdded = null;
            OnReactionRemoved = null;
        }
    }
}
