using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Injhinuity.Client.Discord.Entities;
using Injhinuity.Client.Discord.Services;
using Injhinuity.Client.Discord.Wrappers;
using Injhinuity.Client.Extensions;

namespace Injhinuity.Client.Discord.Embeds
{
    public interface IReactionRoleEmbed : IReactionEmbed
    {
        Task<ulong> InitalizeAsync(IInjhinuityCommandContext context);
        Task InitalizeFromExistingAsync(IUserMessage message);
        Task ResetAsync();
    }

    public class ReactionRoleEmbed : IReactionRoleEmbed, IDisposable
    {
        private readonly IInjhinuityDiscordClient _discordClient;

        private IUserMessage? _message;
        private IMessageReactionWrapper? _wrapper;

        public IEnumerable<UserReactionButton> Buttons { get; init; }
        public IEmbedContent Content { get; init; }

        public ReactionRoleEmbed(IInjhinuityDiscordClient discordClient, IEnumerable<UserReactionButton> buttons, IEmbedContent content)
        {
            _discordClient = discordClient;
            Buttons = buttons;
            Content = content;
        }

        public async Task<ulong> InitalizeAsync(IInjhinuityCommandContext context)
        {
            var content = Content.Get();

            _message = await context.Channel.SendEmbedMessageAsync(content);
            await _message.AddReactionsAsync(Buttons.Select(x => x.Emote).ToArray());

            _wrapper = _message.OnReaction(_discordClient, OnReactionHandlerAsync, OnReactionHandlerAsync);

            return _message.Id;
        }

        public async Task InitalizeFromExistingAsync(IUserMessage message)
        {
            _message = message;
            var content = Content.Get();

            await _message.ModifyAsync(x => x.Embed = content.Build());
            await SyncReactionsAsync();

            _wrapper = _message.OnReaction(_discordClient, OnReactionHandlerAsync, OnReactionHandlerAsync);
        }

        public async Task ResetAsync()
        {
            Dispose();
            if (_message is not null)
                await _message.DeleteAsync();
        }

        public void Dispose()
        {
            _wrapper?.Dispose();
            GC.SuppressFinalize(this);
        }

        private async Task SyncReactionsAsync()
        {
            if (_message is not null)
            {
                // TODO: Implement once Discord.Net 2.3 is available (RemoveAllReactionsForEmoji())

                /*var reactions = _message.Reactions;
                var emotes = Buttons.Select(x => x.Emote);

                var missingButtons = emotes.Where(x => !reactions.TryGetValue(x, out var _));
                await _message.AddReactionsAsync(missingButtons.ToArray());

                var buttonsToRemove = reactions.Where(x => !emotes.Contains(x.Key)).Select(x => x.Key);
                await _message.RemoveAllReactionsForEmote(buttonsToRemove.ToArray());*/

                await _message.RemoveAllReactionsAsync();
                await _message.AddReactionsAsync(Buttons.Select(x => x.Emote).ToArray());
            }
        }

        private async Task OnReactionHandlerAsync(SocketReaction reaction)
        {
            var button = Buttons.FirstOrDefault(x => x.Emote.Name == reaction.Emote.Name);
            if (button is null || reaction.User.Value.Id == _discordClient.CurrentUser.Id)
                return;

            await button.Task.Invoke(reaction.UserId, reaction.Channel);
        }
    }
}
