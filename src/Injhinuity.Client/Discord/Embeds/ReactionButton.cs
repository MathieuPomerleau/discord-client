using System;
using System.Threading.Tasks;
using Discord;

namespace Injhinuity.Client.Discord.Embeds
{
    public record ReactionButton(IEmote Emote, Func<Task> Task) {}
}
