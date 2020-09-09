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
using Injhinuity.Client.Discord.Embeds;
using Injhinuity.Client.Discord.Entities;
using Injhinuity.Client.Discord.Factories;
using Injhinuity.Client.Discord.Services;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace Injhinuity.Client.Tests.Discord.Services
{
    public class CommandHandlerServiceTests
    {
        private readonly ICommandHandlerService _subject;

        private readonly EmbedBuilder _embedBuilder = new EmbedBuilder();
        private readonly string _stringMessage = "message";

        private readonly IServiceProvider _provider;
        private readonly IInjhinuityDiscordClient _discordClient;
        private readonly IInjhinuityCommandService _commandService;
        private readonly IClientConfig _clientConfig;
        private readonly IAssemblyProvider _assemblyProvider;
        private readonly ICustomCommandHandlerService _customCommandHandler;
        private readonly IInjhinuityCommandContextFactory _commandContextFactory;
        private readonly IInjhinuityCommandContext _context;
        private readonly IInjhinuityCommandResult _commandResult;
        private readonly IMessageChannel _channel;
        private readonly IUserMessage _message;
        private readonly IReactionEmbed _reactionEmbed;

        public CommandHandlerServiceTests()
        {
            _provider = Substitute.For<IServiceProvider>();
            _discordClient = Substitute.For<IInjhinuityDiscordClient>();
            _commandService = Substitute.For<IInjhinuityCommandService>();
            _clientConfig = Substitute.For<IClientConfig>();
            _assemblyProvider = Substitute.For<IAssemblyProvider>();
            _customCommandHandler = Substitute.For<ICustomCommandHandlerService>();
            _commandContextFactory = Substitute.For<IInjhinuityCommandContextFactory>();
            _context = Substitute.For<IInjhinuityCommandContext>();
            _commandResult = Substitute.For<IInjhinuityCommandResult>();
            _channel = Substitute.For<IMessageChannel>();
            _message = Substitute.For<IUserMessage>();
            _reactionEmbed = Substitute.For<IReactionEmbed>();

            _clientConfig.Discord.Returns(new Core.Configuration.DiscordConfig("token", '!'));
            _context.Channel.Returns(_channel);
            _context.Message.Returns(_message);

            _subject = new CommandHandlerService(_provider, _discordClient, _commandService, _clientConfig, _assemblyProvider, _customCommandHandler, _commandContextFactory);
        }

        [Fact]
        public async Task InitializeAsync_ThenRegistersEventsAndAddsModules()
        {
            _assemblyProvider.GetCallingAssembly().Returns(Assembly.GetEntryAssembly());

            await _subject.InitializeAsync();
            
            _assemblyProvider.Received().GetCallingAssembly();
            await _commandService.Received().AddModulesAsync(Arg.Any<Assembly>(), _provider);
        }

        [Fact]
        public void InitializeAsync_WithNullAssembly_ThenThrowsException()
        {
            Func<Task> action = async () => await _subject.InitializeAsync();

            action.Should().Throw<InjhinuityException>().WithMessage("Failed to load entry assembly.");
        }

        [Fact]
        public void OnReady_ThenRegistersMessageEvents()
        {
            _subject.OnReady();

            _discordClient.Received().MessageReceived += Arg.Any<Func<SocketMessage, Task>>();
            _commandService.Received().CommandExecuted += Arg.Any<Func<Optional<CommandInfo>, ICommandContext, IResult, Task>>();
        }

        [Fact]
        public void OnDisconnected_ThenUnregistersMessageEvents()
        {
            _subject.OnDisconnected();

            _discordClient.Received().MessageReceived -= Arg.Any<Func<SocketMessage, Task>>();
            _commandService.Received().CommandExecuted -= Arg.Any<Func<Optional<CommandInfo>, ICommandContext, IResult, Task>>();
        }

        [Fact]
        public async Task MessageReceivedAsync_WithoutPrefix_ThenExits()
        {
            _message.Content.Returns("content");

            await _subject.MessageReceivedAsync(_context);

            await _customCommandHandler.DidNotReceive().TryHandlingCustomCommand(_context, Arg.Any<string>());
            await _commandService.DidNotReceive().ExecuteAsync(default, Arg.Any<int>(), _provider);
        }

        [Fact]
        public async Task MessageReceivedAsync_WithPrefixAndCustomCommandIsHandled_ThenExecutesCustomCommandAndExits()
        {
            _message.Content.Returns("!content");
            _customCommandHandler.TryHandlingCustomCommand(_context, Arg.Any<string>()).Returns(true);

            await _subject.MessageReceivedAsync(_context);

            await _customCommandHandler.Received().TryHandlingCustomCommand(_context, Arg.Any<string>());
            await _commandService.DidNotReceive().ExecuteAsync(default, Arg.Any<int>(), _provider);
        }

        [Fact]
        public async Task MessageReceivedAsync_WithPrefixAndCustomCommandIsNotHandled_ThenExecutesCommand()
        {
            _message.Content.Returns("!content");
            _customCommandHandler.TryHandlingCustomCommand(_context, Arg.Any<string>()).Returns(false);

            await _subject.MessageReceivedAsync(_context);

            await _commandService.Received().ExecuteAsync(default, Arg.Any<int>(), _provider);
        }

        [Fact]
        public async Task CommandExecutedAsync_WithReactionEmbedInResult_ThenItInitializesAndExits()
        {
            _commandResult.ReactionEmbed.Returns(_reactionEmbed);
            _commandResult.EmbedBuilder.ReturnsNull();
            _commandResult.Message.ReturnsNull();

            await _subject.CommandExecutedAsync(_context, _commandResult);

            await _reactionEmbed.Received().InitializeAsync(_context);
            await _channel.DidNotReceive().SendMessageAsync(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<Embed>());
            await _channel.DidNotReceive().SendMessageAsync(_stringMessage);
        }

        [Fact]
        public async Task CommandExecutedAsync_WithAnEmbedBuilderInResult_ThenItSendsItAndExits()
        {
            _commandResult.ReactionEmbed.ReturnsNull();
            _commandResult.EmbedBuilder.Returns(_embedBuilder);
            _commandResult.Message.ReturnsNull();

            await _subject.CommandExecutedAsync(_context, _commandResult);

            await _channel.Received().SendMessageAsync(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<Embed>());
            await _reactionEmbed.DidNotReceive().InitializeAsync(_context);
            await _channel.DidNotReceive().SendMessageAsync(_stringMessage);
        }

        [Fact]
        public async Task CommandExecutedAsync_WithAMessageInResult_ThenItSendsItAndExits()
        {
            _commandResult.ReactionEmbed.ReturnsNull();
            _commandResult.EmbedBuilder.ReturnsNull();
            _commandResult.Message.Returns(_stringMessage);

            await _subject.CommandExecutedAsync(_context, _commandResult);

            await _channel.Received().SendMessageAsync(_stringMessage);
            await _reactionEmbed.DidNotReceive().InitializeAsync(_context);
            await _channel.DidNotReceive().SendMessageAsync(Arg.Any<string>(), Arg.Any<bool>(), Arg.Is<Embed>(x => x != null));
        }
    }
}
