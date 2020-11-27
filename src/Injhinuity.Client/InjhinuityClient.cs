using System.Threading;
using System.Threading.Tasks;
using Discord;
using Injhinuity.Client.Core.Configuration;
using Injhinuity.Client.Discord.Factories;
using Injhinuity.Client.Discord.Services;
using Microsoft.Extensions.Logging;

namespace Injhinuity.Client
{
    public interface IInjhinuityClient
    {
        Task RunAsync(bool shouldBlock = true);
        Task OnReadyAsync();
        Task OnDisconnectedAsync();
        Task LogAsync(LogMessage logMessage, string tag);
    }

    public class InjhinuityClient : IInjhinuityClient
    {
        private readonly ILogger _logger;
        private readonly IClientConfig _clientConfig;
        private readonly IInjhinuityDiscordClient _discordClient;
        private readonly IInjhinuityCommandService _commandService;
        private readonly ICommandHandlerService _commandHandlerService;
        private readonly IActivityFactory _activityFactory;
        private readonly IInitialStartupHandler _initialStartupHandler;

        public InjhinuityClient(ILogger<InjhinuityClient> logger, IClientConfig clientConfig, IInjhinuityDiscordClient discordClient, 
            IInjhinuityCommandService commandService, ICommandHandlerService commandHandlerService, IActivityFactory activityFactory,
            IInitialStartupHandler initialStartupHandler)
        {
            _logger = logger;
            _clientConfig = clientConfig;
            _discordClient = discordClient;
            _commandService = commandService;
            _commandHandlerService = commandHandlerService;
            _activityFactory = activityFactory;
            _initialStartupHandler = initialStartupHandler;
        }

        public async Task RunAsync(bool shouldBlock = true)
        {
            _logger.LogInformation($"Launching Injhinuity version {_clientConfig.Version.VersionNo}.");
            await RegisterClientAsync();

            if (shouldBlock)
                await Task.Delay(Timeout.Infinite);
        }

        private async Task RegisterClientAsync()
        {
            _commandService.Log += (message) => LogAsync(message, "[Command Service]");
            _discordClient.Log += (message) => LogAsync(message, "[Discord]");

            _discordClient.Ready += OnReadyAsync;
            _discordClient.Disconnected += _ => OnDisconnectedAsync();

            await _commandHandlerService.InitializeAsync();
            await _discordClient.LoginAsync(TokenType.Bot, _clientConfig.Discord.Token);
            await _discordClient.StartAsync();
            
            await _discordClient.SetActivityAsync(_activityFactory.CreatePlayingStatus($"version {_clientConfig.Version.VersionNo}"));
        }

        public async Task OnReadyAsync()
        {
            _commandHandlerService.OnReady();
            await _initialStartupHandler.ExecuteColdStartupAsync();
            await _initialStartupHandler.ExecuteWarmStartupAsync();
        }

        public Task OnDisconnectedAsync()
        {
            _commandHandlerService.OnDisconnected();
            return Task.CompletedTask;
        }

        public Task LogAsync(LogMessage logMessage, string tag)
        {
            _logger.LogInformation($"{tag} {logMessage.ToString()}.");
            return Task.CompletedTask;
        }
    }
}
