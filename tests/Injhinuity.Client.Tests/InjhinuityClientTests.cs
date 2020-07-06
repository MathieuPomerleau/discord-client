﻿using System;
using System.Threading.Tasks;
using AutoFixture;
using Discord;
using Injhinuity.Client.Core.Configuration;
using Injhinuity.Client.Discord.Factory;
using Injhinuity.Client.Discord.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Injhinuity.Client.Tests
{
    public class InjhinuityClientTests
    {
        private static readonly IFixture _fixture = new Fixture();
        
        private readonly LogMessage _logMessage = new LogMessage(LogSeverity.Info, _fixture.Create<string>(), _fixture.Create<string>());
        private readonly Core.Configuration.DiscordConfig _discordConfig = new Core.Configuration.DiscordConfig("token", '!');
        private readonly Game _activity = new Game(_fixture.Create<string>());
        private readonly string _versionNo = _fixture.Create<string>();

        private readonly IInjhinuityClient _subject;

        private readonly ILogger<InjhinuityClient> _consoleLogger;
        private readonly IClientConfig _clientConfig;
        private readonly IInjhinuityDiscordClient _discordClient;
        private readonly IInjhinuityCommandService _commandService;
        private readonly ICommandHandlerService _commandHandlerService;
        private readonly IActivityFactory _activityFactory;

        public InjhinuityClientTests()
        {
            _consoleLogger = Substitute.For<ILogger<InjhinuityClient>>();
            _clientConfig = Substitute.For<IClientConfig>();
            _discordClient = Substitute.For<IInjhinuityDiscordClient>();
            _commandService = Substitute.For<IInjhinuityCommandService>();
            _commandHandlerService = Substitute.For<ICommandHandlerService>();
            _activityFactory = Substitute.For<IActivityFactory>();

            
            _activityFactory.CreatePlayingStatus(Arg.Any<string>()).Returns(_activity);
            _clientConfig.Version.Returns(new VersionConfig(_versionNo));
            _clientConfig.Discord.Returns(_discordConfig);

            _subject = new InjhinuityClient(_consoleLogger, _clientConfig, _discordClient,
                _commandService, _commandHandlerService, _activityFactory);
        }

        [Fact]
        public async Task RunAsync_WhenCalled_ThenStartupRegisterDiscordServicesAndLogin()
        {
            await _subject.RunAsync(false);

            _consoleLogger.Received().LogInformation($"Launching Injhinuity version {_versionNo}.");

            _commandService.Received().Log += Arg.Any<Func<LogMessage, Task>>();
            _discordClient.Received().Log += Arg.Any<Func<LogMessage, Task>>();
            _discordClient.Received().LoggedIn += Arg.Any<Func<Task>>();
            _discordClient.Received().Ready += Arg.Any<Func<Task>>();

            await _discordClient.Received().LoginAsync(TokenType.Bot, _discordConfig.Token);
            await _discordClient.Received().StartAsync();
        }

        [Fact]
        public async Task OnReadyAsync_WhenCalled_ThenStartupRegisterDiscordServicesAndLogin()
        {
            await _subject.OnReadyAsync();

            await _commandHandlerService.Received().InitializeAsync();
            await _discordClient.Received().SetActivityAsync(_activity);
        }

        [Fact]
        public async Task LogAsync_WhenCalled_ThenStartupRegisterDiscordServicesAndLogin()
        {
            await _subject.LogAsync(_logMessage);

            _consoleLogger.Received().LogInformation($"[Discord] {_logMessage.ToString()}.");
        }

        [Fact]
        public async Task LoggedInAsync_WhenCalled_ThenStartupRegisterDiscordServicesAndLogin()
        {
            await _subject.LoggedInAsync();

            _consoleLogger.Received().LogInformation($"[Discord] Logged in.");
        }
    }
}
