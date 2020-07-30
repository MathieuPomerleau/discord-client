using Discord;

namespace Injhinuity.Client.Discord.Factories
{
    public interface IActivityFactory
    {
        IActivity CreatePlayingStatus(string name);
    }

    public class ActivityFactory : IActivityFactory
    {
        public IActivity CreatePlayingStatus(string name) =>
            new Game(name, ActivityType.Playing);
    }
}
