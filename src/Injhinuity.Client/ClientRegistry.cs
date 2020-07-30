using Injhinuity.Client.Core.Interfaces;
using Injhinuity.Client.Discord.Builders;
using Injhinuity.Client.Discord.Managers;
using Injhinuity.Client.Discord.Factories;
using Injhinuity.Client.Discord.Services;
using Microsoft.Extensions.DependencyInjection;
using Injhinuity.Client.Services.Requesters;
using Injhinuity.Client.Services.Api;
using Injhinuity.Client.Services.Factories;
using Injhinuity.Client.Services.EmbedFactories;

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
                .AddTransient<IInjhinuityEmbedBuilder, InjhinuityEmbedBuilder>()
                .AddSingleton<ICommandHandlerService, CommandHandlerService>()
                .AddSingleton<ICustomCommandHandlerService, CustomCommandHandlerService>()
                .AddSingleton<IChannelManager, ChannelManager>()
                .AddSingleton<IActivityFactory, ActivityFactory>()
                .AddTransient<ICommandResultBuilder, CommandResultBuilder>()
                .AddTransient<IApiGateway, ApiGateway>()
                .AddTransient<IApiReponseDeserializer, ApiResponseDeserializer>()
                .AddTransient<IApiUrlProvider, ApiUrlProvider>()
                .AddTransient<ICommandRequester, CommandRequester>()
                .AddTransient<ICommandPackageFactory, CommandPackageFactory>()
                .AddTransient<ICommandEmbedFactory, CommandEmbedFactory>()
                .AddTransient<IInformationEmbedFactory, InformationEmbedFactory>();
        }
    }
}
