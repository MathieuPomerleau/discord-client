using System.Threading.Tasks;
using Injhinuity.Client.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Injhinuity.Client
{
    public interface IInjhinuityClient
    {
        Task RunAsync(bool shouldBlock = false);
    }

    public class InjhinuityClient : IInjhinuityClient
    {
        private readonly IServiceCollection _services;
        private readonly ILogger _logger;
        private readonly IClientConfig _clientConfig;

        public InjhinuityClient(IServiceCollection services, ILogger<InjhinuityClient> logger, IClientConfig clientConfig)
        {
            _services = services;
            _logger = logger;
            _clientConfig = clientConfig;
        }

        public async Task RunAsync(bool shouldBlock = true)
        {
            // First setups
            RegisterDependencies();

            // Launch
            _logger.LogInformation($"Launching Injhinuity version {_clientConfig?.Version?.VersionNo ?? "unknown"}");

            // Block
            if (shouldBlock)
                await Task.Delay(-1);
        }

        private void RegisterDependencies()
        {
            _services.AddSingleton<IInjhinuityClient>(this);
        }
    }
}
