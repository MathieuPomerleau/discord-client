using Injhinuity.Client.Core.Interfaces;
using Injhinuity.Client.Discord.Builders;
using Injhinuity.Client.Discord.Factories;
using Injhinuity.Client.Discord.Services;
using Microsoft.Extensions.DependencyInjection;
using Injhinuity.Client.Services.Requesters;
using Injhinuity.Client.Services.Api;
using Injhinuity.Client.Services.Factories;
using Injhinuity.Client.Services.Mappers;
using Injhinuity.Client.Discord.Embeds.Factories;
using Injhinuity.Client.Discord.Converters;

namespace Injhinuity.Client
{
    public class ClientRegistry : IRegistry
    {
        public void Register(IServiceCollection services)
        {
            // Base services
            services.AddSingleton<IInjhinuityMapper, InjhinuityMapper>()
                .AddSingleton<IInjhinuityClient, InjhinuityClient>()
                .AddSingleton<IInjhinuityDiscordClient, InjhinuityDiscordClient>()
                .AddSingleton<IInjhinuityCommandService, InjhinuityCommandService>()
                .AddSingleton<ICommandHandlerService, CommandHandlerService>()
                .AddSingleton<ICustomCommandHandlerService, CustomCommandHandlerService>()
                .AddSingleton<IReactionRoleEmbedService, ReactionRoleEmbedService>()
                .AddSingleton<IInitialStartupHandler, InitialStartupHandler>()
                .AddTransient<ICommandExclusionService, CommandExclusionService>();

            // Api services
            services.AddTransient<IApiGateway, ApiGateway>()
                .AddTransient<IApiReponseDeserializer, ApiResponseDeserializer>()
                .AddTransient<IApiUrlProvider, ApiUrlProvider>()
                .AddTransient<ICommandRequester, CommandRequester>()
                .AddTransient<IRoleRequester, RoleRequester>()
                .AddTransient<IGuildRequester, GuildRequester>();

            // Builders
            services.AddTransient<IInjhinuityEmbedBuilder, InjhinuityEmbedBuilder>()
                .AddTransient<IReactionEmbedBuilder, ReactionEmbedBuilder>()
                .AddTransient<ICommandResultBuilder, CommandResultBuilder>();

            // Factories
            services.AddTransient<IActivityFactory, ActivityFactory>()
                .AddTransient<IReactionEmbedFactory, ReactionEmbedFactory>()
                .AddTransient<IInjhinuityCommandContextFactory, InjhinuityCommandContextFactory>()
                .AddTransient<ICommandBundleFactory, CommandBundleFactory>()
                .AddTransient<IRoleBundleFactory, RoleBundleFactory>()
                .AddTransient<IGuildBundleFactory, GuildBundleFactory>()
                .AddTransient<IEmbedBuilderFactoryProvider, EmbedBuilderFactoryProvider>()
                .AddTransient<ICommandEmbedBuilderFactory, CommandEmbedBuilderFactory>()
                .AddTransient<IInformationEmbedBuilderFactory, InformationEmbedBuilderFactory>()
                .AddTransient<IRoleEmbedBuilderFactory, RoleEmbedBuilderFactory>()
                .AddTransient<IAdminEmbedBuilderFactory, AdminEmbedBuilderFactory>()
                .AddTransient<IPermissionEmbedBuilderFactory, PermissionEmbedBuilderFactory>();

            // Misc
            services.AddTransient<ILogSeverityConverter, LogSeverityConverter>();
        }
    }
}
