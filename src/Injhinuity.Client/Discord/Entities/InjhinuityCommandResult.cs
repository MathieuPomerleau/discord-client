using Discord;
using Discord.Commands;
using Injhinuity.Client.Discord.Embeds;

namespace Injhinuity.Client.Discord.Entities
{
    public interface IInjhinuityCommandResult
    {
        EmbedBuilder? EmbedBuilder { get; }
        IReactionEmbed? ReactionEmbed { get; }
        string? Message { get; }
    }

    public class InjhinuityCommandResult : RuntimeResult, IInjhinuityCommandResult
    {
        public EmbedBuilder? EmbedBuilder { get; }
        public IReactionEmbed? ReactionEmbed { get; }
        public string? Message { get; }

        public InjhinuityCommandResult(CommandError? error = null, string? message = null, EmbedBuilder? embedBuilder = null, IReactionEmbed? reactionEmbed = null,
            string? reason = null) : base(error, reason)
        {
            EmbedBuilder = embedBuilder;
            ReactionEmbed = reactionEmbed;
            Message = message;
        }
    }
}
