using System;
using System.Threading.Tasks;
using Discord;

namespace Injhinuity.Client.Discord.Embeds
{
    public class ReactionButton
    {
        public IEmote Emote { get; set; }
        public Func<Task> Task { get; set; }
        
        public ReactionButton(IEmote emote, Func<Task> task)
        {
            Emote = emote;
            Task = task;
        }
    }
}
