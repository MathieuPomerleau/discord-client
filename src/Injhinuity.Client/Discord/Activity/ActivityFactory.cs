﻿using Discord;

namespace Injhinuity.Client.Discord.Activity
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
