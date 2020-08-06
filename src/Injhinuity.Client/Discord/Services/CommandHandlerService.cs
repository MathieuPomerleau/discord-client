using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Injhinuity.Client.Core;
using Injhinuity.Client.Core.Configuration;
using Injhinuity.Client.Core.Exceptions;
using Injhinuity.Client.Discord.Entities;
using Injhinuity.Client.Discord.Factories;
using Injhinuity.Client.Extensions;

namespace Injhinuity.Client.Discord.Services
{
    public interface ICommandHandlerService
    {
        Task InitializeAsync();
        void OnReady();
        void OnDisconnected();
        Task MessageReceivedAsync(IInjhinuityCommandContext context);
        Task CommandExecutedAsync(IInjhinuityCommandContext context, IInjhinuityCommandResult result);
    }

    public class CommandHandlerService : ICommandHandlerService
    {
        private readonly IServiceProvider _provider;
        private readonly IInjhinuityDiscordClient _discordClient;
        private readonly IInjhinuityCommandService _commandService;
        private readonly IClientConfig _clientConfig;
        private readonly IAssemblyProvider _assemblyProvider;
        private readonly ICustomCommandHandlerService _customCommandHandler;
        private readonly IInjhinuityCommandContextFactory _commandContextFactory;

        public CommandHandlerService(IServiceProvider provider, IInjhinuityDiscordClient discordClient, IInjhinuityCommandService commandService,
            IClientConfig clientConfig, IAssemblyProvider assemblyProvider, ICustomCommandHandlerService customCommandHandler,
            IInjhinuityCommandContextFactory commandContextFactory)
        {
            _provider = provider;
            _discordClient = discordClient;
            _commandService = commandService;
            _clientConfig = clientConfig;
            _assemblyProvider = assemblyProvider;
            _customCommandHandler = customCommandHandler;
            _commandContextFactory = commandContextFactory;
        }

        public async Task InitializeAsync()
        {
            await _commandService.AddModulesAsync(_assemblyProvider.GetCallingAssembly() ?? throw new InjhinuityException("Failed to load entry assembly."), _provider);
        }

        public void OnReady()
        {
            _discordClient.MessageReceived += MessageReceivedDecoratorAsync;
            _commandService.CommandExecuted += CommandExecutedDecoratorAsync;
        }

        public void OnDisconnected()
        {
            _discordClient.MessageReceived -= MessageReceivedDecoratorAsync;
            _commandService.CommandExecuted -= CommandExecutedDecoratorAsync;
        }

        private Task MessageReceivedDecoratorAsync(SocketMessage rawMessage)
        {
            if (!(rawMessage is SocketUserMessage message) || message.Source != MessageSource.User)
                return Task.CompletedTask;

            return MessageReceivedAsync(_commandContextFactory.Create(_discordClient, new InjhinuityUserMessage(message)));
        }

        private Task CommandExecutedDecoratorAsync(Optional<CommandInfo> _, ICommandContext context, IResult result)
        {
            if (!(result is InjhinuityCommandResult injhinuityResult))
                return Task.CompletedTask;

            return CommandExecutedAsync(_commandContextFactory.Create(context), injhinuityResult);
        }

        public async Task MessageReceivedAsync(IInjhinuityCommandContext context)
        {
            var argPos = 0;
            if (!context.Message.HasCharPrefix(_clientConfig.Discord.Prefix, ref argPos))
                return;

            if (await _customCommandHandler.TryHandlingCustomCommand(context, context.Message.Content[1..]))
                return;

            await _commandService.ExecuteAsync(context.GetSocketContext(), argPos, _provider);
        }

        public async Task CommandExecutedAsync(IInjhinuityCommandContext context, IInjhinuityCommandResult result)
        {
            if (!(result.ReactionEmbed is null))
            {
                await result.ReactionEmbed.InitializeAsync(context);
                return;
            }

            if (!(result.EmbedBuilder is null))
            {
                await context.Channel.SendEmbedMessageAsync(result.EmbedBuilder);
                return;
            }

            if (!(result.Message is null))
            {
                await context.Channel.SendMessageAsync(result.Message);
                return;
            }
        }
    }
}
