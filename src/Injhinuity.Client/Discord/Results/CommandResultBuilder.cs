using Discord;
using Discord.Commands;

namespace Injhinuity.Client.Discord.Results
{
    public interface ICommandResultBuilder
    {
        ICommandResultBuilder WithError(CommandError error);
        ICommandResultBuilder WithMessage(string message);
        ICommandResultBuilder WithEmbed(Embed embed);
        ICommandResultBuilder WithReason(string reason);
        InjhinuityCommandResult Build();
    }

    public class CommandResultBuilder : ICommandResultBuilder
    {
        private CommandError? _commandError;
        private Embed? _embed;
        private string? _message;
        private string? _reason;

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

        public ICommandResultBuilder WithEmbed(Embed embed)
        {
            _embed = embed;
            return this;
        }

        public ICommandResultBuilder WithReason(string reason)
        {
            _reason = reason;
            return this;
        }

        public InjhinuityCommandResult Build() =>
            new InjhinuityCommandResult(_commandError, _message, _embed, _reason);
    }
}
