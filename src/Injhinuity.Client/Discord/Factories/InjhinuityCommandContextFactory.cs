using Discord.Commands;
using Injhinuity.Client.Discord.Entities;
using Injhinuity.Client.Discord.Services;

namespace Injhinuity.Client.Discord.Factories
{
    public interface IInjhinuityCommandContextFactory
    {
        IInjhinuityCommandContext Create(IInjhinuityDiscordClient discordClient, IInjhinuityUserMessage message);
        IInjhinuityCommandContext Create(ICommandContext context);
    }

    public class InjhinuityCommandContextFactory : IInjhinuityCommandContextFactory
    {
        public IInjhinuityCommandContext Create(IInjhinuityDiscordClient discordClient, IInjhinuityUserMessage message) =>
            new InjhinuityCommandContext(discordClient, message);

        public IInjhinuityCommandContext Create(ICommandContext context) =>
            new InjhinuityCommandContext(context);
    }
}
