using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Injhinuity.Client.Core;
using Injhinuity.Client.Core.Configuration;
using Injhinuity.Client.Core.Configuration.Options;
using Injhinuity.Client.Core.Exceptions;
using Injhinuity.Client.Discord.Converters;
using Injhinuity.Client.Discord.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Injhinuity.Client
{
    class Program
    {
        private static readonly IClientConfigMapper _configMapper = new ClientConfigMapper();

        public static async Task Main()
        {
            var services = new ServiceCollection();

            ConfigureConfiguration(services);
            ConfigureLogging(services);
            ConfigureHttpClient(services);
            Register(services);

            using var provider = services.BuildServiceProvider();
            await provider.GetRequiredService<IInjhinuityClient>().RunAsync();
        }

        private static void ConfigureConfiguration(IServiceCollection services)
        {
            var options = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", false, true)
               .AddJsonFile($"appsettings.dev.json", true)
               .AddEnvironmentVariables()
               .Build()
               .GetSection(IClientOptions.SectionName)
               .Get<ClientOptions>();

            services.AddSingleton(_configMapper.MapFromNullableOptions(options));
            services.AddSingleton<IInjhinuityDiscordClientConfig>(provider =>
            {
                var config = provider.GetService<IClientConfig>();
                var converter = provider.GetService<ILogSeverityConverter>();

                if (config is null || converter is null)
                    throw new InjhinuityException("Missing required services for creation of discord client configuration.");

                var discordConfig = new DiscordSocketConfig
                {
                    LogLevel = converter.FromLogLevel(config.Logging.DiscordLogLevel)
                };

                return new InjhinuityDiscordClientConfig(discordConfig);
            });
        }

        private static void ConfigureLogging(IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();

            services.AddLogging(opt => opt.AddConsole())
                .Configure<LoggerFilterOptions>(opt => {
                    opt.MinLevel = provider.GetRequiredService<IClientConfig>().Logging.AppLogLevel;
                });
        }

        private static void ConfigureHttpClient(IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();

            services.AddHttpClient("injhinuity", http =>
            {
                http.BaseAddress = new Uri(provider.GetRequiredService<IClientConfig>().Api.BaseUrl);
            });
        }

        private static void Register(IServiceCollection services)
        {
            new CoreRegistry().Register(services);
            new ClientRegistry().Register(services);

            services.AddSingleton(services);
        }
    }
}
