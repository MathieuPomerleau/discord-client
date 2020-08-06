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
    public interface IReactionEmbed
    {
        Task InitializeAsync(IInjhinuityCommandContext context);
    }

    public class ReactionEmbed : IReactionEmbed
    {
        private readonly Timer _timer;
        private readonly IInjhinuityDiscordClient _discordClient;

        private IUserMessage? _message;
        private IUserMessageReactionWrapper? _wrapper;

        public IEnumerable<ReactionButton> Buttons { get; set; }
        public IReactionEmbedContent Content { get; set; }

        public ReactionEmbed(long lifetimeInSeconds, IInjhinuityDiscordClient discordClient, IEnumerable<ReactionButton> buttons,
            IReactionEmbedContent content)
        {
            _discordClient = discordClient;
            Buttons = buttons;
            Content = content;

            _timer = new Timer(lifetimeInSeconds * 1000)
            {
                Enabled = true,
                AutoReset = false
            };
            _timer.Elapsed += OnLifetimePassed;
            _timer.Start();
        }

        public async Task InitializeAsync(IInjhinuityCommandContext context)
        {
            var content = Content.Get();
            
            _message = await context.Channel.SendEmbedMessageAsync(content);
            await _message.AddReactionsAsync(Buttons.Select(x => x.Emote).ToArray());

            _wrapper = _message.OnReaction(_discordClient, OnReactionHandlerAsync, OnReactionHandlerAsync);
        }

        private void ForceContentUpdate() =>
            _message?.ModifyAsync(x => x.Embed = Content.Get().Build());

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
