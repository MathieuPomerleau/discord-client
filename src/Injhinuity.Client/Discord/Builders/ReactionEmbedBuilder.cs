using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Injhinuity.Client.Core.Exceptions;
using Injhinuity.Client.Discord.Embeds;
using Injhinuity.Client.Discord.Services;

namespace Injhinuity.Client.Discord.Builders
{
    public interface IReactionEmbedBuilder
    {
        IReactionEmbedBuilder Create();
        IReactionEmbedBuilder WithButton(IEmote emote, Func<Task> task);
        IReactionEmbedBuilder WithButtons(IEnumerable<ReactionButton> buttons);
        IReactionEmbedBuilder WithUserButtons(IEnumerable<UserReactionButton> buttons);
        IReactionEmbedBuilder WithContent(IEmbedContent content);
        IReactionEmbedBuilder WithLifetime(long lifetimeInSeconds);
        IPageReactionEmbed BuildPage();
        IReactionRoleEmbed BuildRole();
    }

    public class ReactionEmbedBuilder : IReactionEmbedBuilder
    {
        private readonly IInjhinuityDiscordClient _discordClient;

        private readonly List<ReactionButton> _buttons = new List<ReactionButton>();
        private readonly List<UserReactionButton> _userButtons = new List<UserReactionButton>();
        private IEmbedContent? _content;
        private long? _lifetimeInSeconds;

        public ReactionEmbedBuilder(IInjhinuityDiscordClient discordClient)
        {
            _discordClient = discordClient;
        }

        public IReactionEmbedBuilder Create()
        {
            _buttons.Clear();
            _userButtons.Clear();
            _content = null;
            _lifetimeInSeconds = null;
            return this;
        }

        public IReactionEmbedBuilder WithButton(IEmote emote, Func<Task> task)
        {
            _buttons.Add(new ReactionButton(emote, task));
            return this;
        }

        public IReactionEmbedBuilder WithButtons(IEnumerable<ReactionButton> buttons)
        {
            _buttons.AddRange(buttons);
            return this;
        }

        public IReactionEmbedBuilder WithUserButtons(IEnumerable<UserReactionButton> buttons)
        {
            _userButtons.AddRange(buttons);
            return this;
        }

        public IReactionEmbedBuilder WithContent(IEmbedContent content)
        {
            _content = content;
            return this;
        }

        public IReactionEmbedBuilder WithLifetime(long lifetimeInSeconds)
        {
            _lifetimeInSeconds = lifetimeInSeconds;
            return this;
        }

        public IPageReactionEmbed BuildPage()
        {
            if (_content is null || _buttons.Count == 0)
                throw new InjhinuityException("No content or buttons provided for PageReactionEmbed.");

            if (!_lifetimeInSeconds.HasValue)
                throw new InjhinuityException("No lifetime provided for PageReactionEmbed.");

            return new PageReactionEmbed(_lifetimeInSeconds.Value, _discordClient, _buttons, _content);
        }

        public IReactionRoleEmbed BuildRole()
        {
            if (_content is null)
                throw new InjhinuityException("No content provided for RoleReactionEmbed.");

            return new ReactionRoleEmbed(_discordClient, _userButtons, _content);
        }
    }
}
