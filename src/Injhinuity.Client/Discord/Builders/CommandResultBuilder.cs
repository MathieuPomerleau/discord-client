using Discord;
using Discord.Commands;
using Injhinuity.Client.Discord.Embeds;
using Injhinuity.Client.Discord.Entities;

namespace Injhinuity.Client.Discord.Builders
{
    public interface ICommandResultBuilder
    {
        ICommandResultBuilder Create();
        ICommandResultBuilder WithError(CommandError error);
        ICommandResultBuilder WithMessage(string message);
        ICommandResultBuilder WithEmbedBuilder(EmbedBuilder embedBuilder);
        ICommandResultBuilder WithReactionEmbed(IReactionEmbed embed);
        ICommandResultBuilder WithReason(string reason);
        InjhinuityCommandResult Build();
    }

    public class CommandResultBuilder : ICommandResultBuilder
    {
        private CommandError? _commandError;
        private EmbedBuilder? _embedBuilder;
        private IReactionEmbed? _reactionEmbed;
        private string? _message;
        private string? _reason;

        public ICommandResultBuilder Create()
        {
            _commandError = null;
            _embedBuilder = null;
            _reactionEmbed = null;
            _message = null;
            _reason = null;
            return this;
        }

        public ICommandResultBuilder WithError(CommandError error)
        {
            _commandError = error;
            return this;
        }

        public ICommandResultBuilder WithMessage(string message)
        {
            _message = message;
            return this;
        }

        public ICommandResultBuilder WithEmbedBuilder(EmbedBuilder embedBuilder)
        {
            _embedBuilder = embedBuilder;
            return this;
        }

        public ICommandResultBuilder WithReactionEmbed(IReactionEmbed embed)
        {
            _reactionEmbed = embed;
            return this;
        }

        public ICommandResultBuilder WithReason(string reason)
        {
            _reason = reason;
            return this;
        }

        public InjhinuityCommandResult Build() =>
            new InjhinuityCommandResult(_commandError, _message, _embedBuilder, _reactionEmbed, _reason);
    }
}
