using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Injhinuity.Client.Discord.Services;

namespace Injhinuity.Client.Discord.Wrappers
{
    public interface IMessageReactionWrapper : IDisposable
    {
    }

    public class MessageReactionWrapper : IMessageReactionWrapper
    {
        private const long FiveHundredMillis = 500;
        private long _lastReactTimestamp = CurrentUnixMillis();
        private bool _disposing;
        private readonly IMessage _message;
        private readonly IInjhinuityDiscordClient _discordClient;

        private event Func<SocketReaction, Task>? OnReactionAdded;
        private event Func<SocketReaction, Task>? OnReactionRemoved;

        public MessageReactionWrapper(IMessage message, IInjhinuityDiscordClient discordClient, Func<SocketReaction, Task> reactionAdded, Func<SocketReaction, Task> reactionRemoved)
        {
            _message = message;
            _discordClient = discordClient;

            _discordClient.ReactionAdded += ReactionAdded;
            _discordClient.ReactionRemoved += ReactionRemoved;

            OnReactionAdded += reactionAdded;
            OnReactionRemoved += reactionRemoved;
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

            GC.SuppressFinalize(this);
        }

        private Task ReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel messageChannel, SocketReaction reaction)
        {
            if (CooldownHasElasped() && _message.Id == message.Id)
            {
                OnReactionAdded?.Invoke(reaction);
                _lastReactTimestamp = CurrentUnixMillis();
            }

            return Task.CompletedTask;
        }

        private Task ReactionRemoved(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel messageChannel, SocketReaction reaction)
        {
            if (CooldownHasElasped() && _message.Id == message.Id)
            {
                OnReactionRemoved?.Invoke(reaction);
                _lastReactTimestamp = CurrentUnixMillis();
            }

            return Task.CompletedTask;
        }

        private bool CooldownHasElasped() => _lastReactTimestamp + FiveHundredMillis < CurrentUnixMillis();
        private static long CurrentUnixMillis() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}
