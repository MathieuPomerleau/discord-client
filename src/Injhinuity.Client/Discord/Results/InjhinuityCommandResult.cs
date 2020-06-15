using Discord;
using Discord.Commands;

namespace Injhinuity.Client.Discord.Results
{
    public class InjhinuityCommandResult : RuntimeResult
    {
        public Embed? Embed { get; }
        public string? Message { get; }

        public InjhinuityCommandResult(CommandError? error = null, string? message = null, Embed? embed = null, string? reason = null) : base(error, reason)
        {
            Embed = embed;
            Message = message;
        }
    }
}
