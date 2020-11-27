using System;
using System.Threading.Tasks;
using Discord;

namespace Injhinuity.Client.Discord.Embeds
{
    public record UserReactionButton(IEmote Emote, Func<ulong, IMessageChannel, Task> Task) {}
}
