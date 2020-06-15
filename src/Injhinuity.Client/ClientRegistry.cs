using Injhinuity.Client.Core.Interfaces;
using Injhinuity.Client.Discord;
using Injhinuity.Client.Discord.Activities;
using Injhinuity.Client.Discord.Channels;
using Injhinuity.Client.Discord.Embeds;
using Injhinuity.Client.Discord.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Injhinuity.Client
{
    public class ClientRegistry : IRegistry
    {
        public void Register(IServiceCollection services)
        {
            services
                .AddSingleton<IInjhinuityClient, InjhinuityClient>()
                .AddSingleton<IDiscordSocketClient, InjhinuityDiscordSocketClient>()
                .AddSingleton<ICommandService, InjhinuityCommandService>()
                .AddSingleton<ICommandHandlerService, CommandHandlerService>()
                .AddSingleton<IChannelManager, ChannelManager>()
                .AddSingleton<IActivityFactory, ActivityFactory>()
                .AddTransient<ICommandResultBuilder, CommandResultBuilder>()
                .AddTransient<IEmbedBuilder, InjhinuityEmbedBuilder>();
        }
    }
}
