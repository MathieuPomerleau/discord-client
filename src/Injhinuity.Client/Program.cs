using System.Threading.Tasks;
using Injhinuity.Client.Core;
using Injhinuity.Client.Core.Configuration;
using Injhinuity.Client.Core.Configuration.Options;
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
        }

        private static void ConfigureLogging(IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();

            services.AddLogging(opt => opt.AddConsole())
                .Configure<LoggerFilterOptions>(opt => {
                    opt.MinLevel = provider.GetRequiredService<IClientConfig>().Logging.LogLevel;
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
