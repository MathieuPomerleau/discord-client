using Injhinuity.Client.Core.Interfaces;
using Injhinuity.Client.Discord.Builders;
using Injhinuity.Client.Discord.Factories;
using Injhinuity.Client.Discord.Services;
using Microsoft.Extensions.DependencyInjection;
using Injhinuity.Client.Services.Requesters;
using Injhinuity.Client.Services.Api;
using Injhinuity.Client.Services.Factories;
using Injhinuity.Client.Services.Mappers;

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
                .AddSingleton<IInjhinuityMapper, InjhinuityMapper>()
                .AddSingleton<ICommandHandlerService, CommandHandlerService>()
                .AddSingleton<ICustomCommandHandlerService, CustomCommandHandlerService>()
                .AddSingleton<IActivityFactory, ActivityFactory>()
                .AddTransient<IInjhinuityEmbedBuilder, InjhinuityEmbedBuilder>()
                .AddTransient<ICommandResultBuilder, CommandResultBuilder>()
                .AddTransient<IApiGateway, ApiGateway>()
                .AddTransient<IApiReponseDeserializer, ApiResponseDeserializer>()
                .AddTransient<IApiUrlProvider, ApiUrlProvider>()
                .AddTransient<ICommandRequester, CommandRequester>()
                .AddTransient<ICommandBundleFactory, CommandBundleFactory>()
                .AddTransient<ICommandEmbedFactory, CommandEmbedFactory>()
                .AddTransient<IInformationEmbedFactory, InformationEmbedFactory>()
                .AddTransient<ICommandExclusionService, CommandExclusionService>()
                .AddTransient<IReactionEmbedBuilder, ReactionEmbedBuilder>()
                .AddTransient<IReactionEmbedFactory, ReactionEmbedFactory>()
                .AddTransient<IInjhinuityCommandContextFactory, InjhinuityCommandContextFactory>();
        }
    }
}
