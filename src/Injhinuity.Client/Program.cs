using System.Threading.Tasks;
using Injhinuity.Client.Configuration;
using Injhinuity.Client.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Injhinuity.Client
{
    class Program
    {
        public static async Task Main()
        {
            var services = new ServiceCollection();

            ConfigureConfiguration(services);
            ConfigureLogging(services);
            Register(services);

            var logger = services.BuildServiceProvider().GetService<ILogger<InjhinuityClient>>();
            var config = services.BuildServiceProvider().GetService<IClientConfig>();

            await new InjhinuityClient(services, logger, config).RunAsync();
        }

        private static void ConfigureConfiguration(IServiceCollection services)
        {
            var config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", false, true)
               .AddJsonFile($"appsettings.dev.json", true)
               .Build()
               .GetSection("Client")
               .Get<ClientConfig>();

            services.AddSingleton<IClientConfig>(config);
        }

        private static void ConfigureLogging(IServiceCollection services)
        {
            services.AddLogging(opt => opt.AddConsole())
                .Configure<LoggerFilterOptions>(opt => opt.MinLevel = LogLevel.Information);
        }

        private static void Register(IServiceCollection services)
        {
            new CoreRegistry().Register(services);
            new ClientRegistry().Register(services);
        }
    }
}
