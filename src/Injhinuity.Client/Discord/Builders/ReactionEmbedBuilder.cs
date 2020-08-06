using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Injhinuity.Client.Discord.Embeds;
using Injhinuity.Client.Discord.Services;

namespace Injhinuity.Client.Discord.Builders
{
    public interface IReactionEmbedBuilder
    {
        IReactionEmbedBuilder Create();
        IReactionEmbedBuilder WithButton(IEmote emote, Func<Task> task);
        IReactionEmbedBuilder WithContent(IReactionEmbedContent content);
        IReactionEmbedBuilder WithLifetime(long lifetimeInSeconds);
        IReactionEmbed Build();
    }

    public class ReactionEmbedBuilder : IReactionEmbedBuilder
    {
        private readonly IInjhinuityDiscordClient _discordClient;

        private List<ReactionButton> _buttons = new List<ReactionButton>();
        private IReactionEmbedContent? _content;
        private long? _lifetimeInSeconds;

        public ReactionEmbedBuilder(IInjhinuityDiscordClient discordClient)
        {
            _discordClient = discordClient;
        }

        public IReactionEmbedBuilder Create()
        {
            _buttons.Clear();
            _content = null;
            _lifetimeInSeconds = null;
            return this;
        }

        public IReactionEmbedBuilder WithButton(IEmote emote, Func<Task> task)
        {
            _buttons.Add(new ReactionButton(emote, task));
            return this;
        }

        public IReactionEmbedBuilder WithContent(IReactionEmbedContent content)
        {
            _content = content;
            return this;
        }

        public IReactionEmbedBuilder WithLifetime(long lifetimeInSeconds)
        {
            _lifetimeInSeconds = lifetimeInSeconds;
            return this;
        }

        public IReactionEmbed Build()
        {
            if (_lifetimeInSeconds is null)
                _lifetimeInSeconds = 60;

            if (_content is null)
                throw new ArgumentException("No content provided for ReactionEmbed.");

            return new ReactionEmbed(_lifetimeInSeconds.Value, _discordClient, _buttons, _content);
        }
    }
}
