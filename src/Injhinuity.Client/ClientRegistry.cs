using Injhinuity.Client.Core.Interfaces;
using Injhinuity.Client.Discord.Builder;
using Injhinuity.Client.Discord.Channel;
using Injhinuity.Client.Discord.Factory;
using Injhinuity.Client.Discord.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Injhinuity.Client
{
    public class ClientRegistry : IRegistry
    {
        public void Register(IServiceCollection services)
        {
            services
                .AddSingleton<IInjhinuityClient, InjhinuityClient>()
                .AddSingleton<IInjhinuityDiscordClient, InjhinuityDiscordClient>()
                .AddSingleton<IInjhinuityCommandService, InjhinuityCommandService>()
                .AddSingleton<ICommandHandlerService, CommandHandlerService>()
                .AddSingleton<IChannelManager, ChannelManager>()
                .AddSingleton<IActivityFactory, ActivityFactory>()
                .AddTransient<ICommandResultBuilder, CommandResultBuilder>()
                .AddTransient<IInjhinuityEmbedBuilder, InjhinuityEmbedBuilder>();
        }
    }
}
