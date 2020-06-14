using System.Threading;
using System.Threading.Tasks;
using Discord;
using Injhinuity.Client.Core.Configuration;
using Injhinuity.Client.Discord;
using Injhinuity.Client.Discord.Activity;
using Microsoft.Extensions.Logging;

namespace Injhinuity.Client
{
    public interface IInjhinuityClient
    {
        Task RunAsync(bool shouldBlock = true);
        Task OnReadyAsync();
        Task LogAsync(LogMessage logMessage);
        Task LoggedInAsync();
    }

    public class InjhinuityClient : IInjhinuityClient
    {
        private readonly ILogger _logger;
        private readonly IClientConfig _clientConfig;
        private readonly IDiscordSocketClient _discordClient;
        private readonly ICommandService _commandService;
        private readonly ICommandHandlerService _commandHandlerService;
        private readonly IActivityFactory _activityFactory;

        public InjhinuityClient(ILogger<InjhinuityClient> logger, IClientConfig clientConfig, IDiscordSocketClient discordClient, 
            ICommandService commandService, ICommandHandlerService commandHandlerService, IActivityFactory activityFactory)
        {
            _logger = logger;
            _clientConfig = clientConfig;
            _discordClient = discordClient;
            _commandService = commandService;
            _commandHandlerService = commandHandlerService;
            _activityFactory = activityFactory;
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
            _commandService.Log += LogAsync;

            _discordClient.Log += LogAsync;
            _discordClient.LoggedIn += LoggedInAsync;
            _discordClient.Ready += OnReadyAsync;

            await _discordClient.LoginAsync(TokenType.Bot, _clientConfig.Discord.Token);
            await _discordClient.StartAsync();
        }

        public async Task OnReadyAsync()
        {
            await _commandHandlerService.InitializeAsync();
            await _discordClient.SetActivityAsync(_activityFactory.CreatePlayingStatus($"version {_clientConfig.Version.VersionNo}"));
        }

        public Task LogAsync(LogMessage logMessage)
        {
            _logger.LogInformation($"[Discord] {logMessage.ToString()}.");
            return Task.CompletedTask;
        }

        public Task LoggedInAsync()
        {
            _logger.LogInformation($"[Discord] Logged in.");
            return Task.CompletedTask;
        }
    }
}
