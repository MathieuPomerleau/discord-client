using Injhinuity.Client.Core.Interfaces;
using Injhinuity.Client.Discord;
using Injhinuity.Client.Discord.Activity;
using Injhinuity.Client.Discord.Channel;
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
                .AddSingleton<IActivityFactory, ActivityFactory>();
        }
    }
}
