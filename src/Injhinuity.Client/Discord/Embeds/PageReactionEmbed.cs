using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using Discord.WebSocket;
using Injhinuity.Client.Discord.Entities;
using Injhinuity.Client.Discord.Services;
using Injhinuity.Client.Discord.Wrappers;
using Injhinuity.Client.Extensions;

namespace Injhinuity.Client.Discord.Embeds
{
    public interface IPageReactionEmbed : IReactionEmbed
    {
        Task InitalizeAsync(IInjhinuityCommandContext context);
    }

    public class PageReactionEmbed : IPageReactionEmbed
    {
        private readonly IInjhinuityDiscordClient _discordClient;

        private IUserMessage? _message;
        private IMessageReactionWrapper? _wrapper;

        public IEnumerable<ReactionButton> Buttons { get; init; }
        public IEmbedContent Content { get; init; }

        public PageReactionEmbed(long lifetimeInSeconds, IInjhinuityDiscordClient discordClient,
            IEnumerable<ReactionButton> buttons, IEmbedContent content)
        {
            _discordClient = discordClient;
            Buttons = buttons;
            Content = content;

            var timer = new Timer(lifetimeInSeconds * 1000)
            {
                Enabled = true,
                AutoReset = false
            };
            timer.Elapsed += OnLifetimePassed;
            timer.Start();
        }

        public async Task InitalizeAsync(IInjhinuityCommandContext context)
        {
            var content = Content.Get();
            
            _message = await context.Channel.SendEmbedMessageAsync(content);
            await _message.AddReactionsAsync(Buttons.Select(x => x.Emote).ToArray());

            _wrapper = _message.OnReaction(_discordClient, OnReactionHandlerAsync, OnReactionHandlerAsync);
        }

        private void ForceContentUpdate() => _message?.ModifyAsync(x => x.Embed = Content.Get().Build());

        private void OnLifetimePassed(object sender, ElapsedEventArgs e)
        {
            _wrapper?.Dispose();
            _message?.RemoveAllReactionsAsync();
        }

        private async Task OnReactionHandlerAsync(SocketReaction reaction)
        {
            var button = Buttons.FirstOrDefault(x => x.Emote.Name == reaction.Emote.Name);
            if (button is null || reaction.User.Value.Id == _discordClient.CurrentUser.Id)
                return;

            await button.Task.Invoke();
            ForceContentUpdate();
        }
    }
}
