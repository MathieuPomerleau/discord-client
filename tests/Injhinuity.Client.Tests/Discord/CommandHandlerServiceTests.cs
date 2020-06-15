using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FluentAssertions;
using Injhinuity.Client.Core;
using Injhinuity.Client.Core.Configuration;
using Injhinuity.Client.Core.Exceptions;
using Injhinuity.Client.Discord;
using Injhinuity.Client.Discord.Channels;
using NSubstitute;
using Xunit;

namespace Injhinuity.Client.Tests.Discord
{
    public class CommandHandlerServiceTests
    {
        private readonly ICommandHandlerService _subject;

        private readonly IServiceProvider _provider;
        private readonly IDiscordSocketClient _discordClient;
        private readonly ICommandService _commandService;
        private readonly IClientConfig _clientConfig;
        private readonly IAssemblyProvider _assemblyProvider;
        private readonly IChannelManager _channelManager;

        public CommandHandlerServiceTests()
        {
            _provider = Substitute.For<IServiceProvider>();
            _discordClient = Substitute.For<IDiscordSocketClient>();
            _commandService = Substitute.For<ICommandService>();
            _clientConfig = Substitute.For<IClientConfig>();
            _assemblyProvider = Substitute.For<IAssemblyProvider>();
            _channelManager = Substitute.For<IChannelManager>();

            _subject = new CommandHandlerService(_provider, _discordClient, _commandService, _clientConfig, _assemblyProvider, _channelManager);
        }

        [Fact]
        public async Task InitializeAsync_WhenCalled_ThenRegistersEventsAndAddsModules()
        {
            _assemblyProvider.GetCallingAssembly().Returns(Assembly.GetEntryAssembly());

            await _subject.InitializeAsync();

            _commandService.Received().CommandExecuted += Arg.Any<Func<Optional<CommandInfo>, ICommandContext, IResult, Task>>();
            _discordClient.Received().MessageReceived += Arg.Any<Func<SocketMessage, Task>>();
            _assemblyProvider.Received().GetCallingAssembly();
            await _commandService.Received().AddModulesAsync(Arg.Any<Assembly>(), _provider);
        }

        [Fact]
        public void InitializeAsync_WhenCalledAndAssemblyIsNull_ThenThrowsException()
        {
            Func<Task> action = async () => await _subject.InitializeAsync();

            action.Should().Throw<InjhinuityException>().WithMessage("Failed to load entry assembly.");
        }
    }
}
