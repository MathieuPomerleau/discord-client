﻿using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Injhinuity.Client.Discord.Services;
using Injhinuity.Client.Discord.Wrappers;

namespace Injhinuity.Client.Extensions
{
    public static class Extensions
    {
        public static IMessageReactionWrapper OnReaction(this IMessage message, IInjhinuityDiscordClient client,
            Func<SocketReaction, Task> reactionAdded, Func<SocketReaction, Task> reactionRemoved) =>
                new MessageReactionWrapper(message, client, reactionAdded, reactionRemoved);

        public static Task<IUserMessage> SendEmbedMessageAsync(this IMessageChannel channel, EmbedBuilder embedBuilder) =>
            channel.SendMessageAsync(string.Empty, false, embedBuilder.Build());
    }
}
