﻿using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Injhinuity.Client.Core;
using Injhinuity.Client.Core.Configuration;
using Injhinuity.Client.Core.Exceptions;
using Injhinuity.Client.Discord.Managers;
using Injhinuity.Client.Discord.Entities;

namespace Injhinuity.Client.Discord.Services
{
    public interface ICommandHandlerService
    {
        Task InitializeAsync();
        Task MessageReceivedAsync(SocketMessage rawMessage);
        Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result);
    }

    public class CommandHandlerService : ICommandHandlerService
    {
        private readonly IServiceProvider _provider;
        private readonly IInjhinuityDiscordClient _discordClient;
        private readonly IInjhinuityCommandService _commandService;
        private readonly IClientConfig _clientConfig;
        private readonly IAssemblyProvider _assemblyProvider;
        private readonly IChannelManager _channelManager;
        private readonly ICustomCommandHandlerService _customCommandHandler;

        public CommandHandlerService(IServiceProvider provider, IInjhinuityDiscordClient discordClient, IInjhinuityCommandService commandService,
            IClientConfig clientConfig, IAssemblyProvider assemblyProvider, IChannelManager channelManager, ICustomCommandHandlerService customCommandHandler)
        {
            _provider = provider;
            _discordClient = discordClient;
            _commandService = commandService;
            _clientConfig = clientConfig;
            _assemblyProvider = assemblyProvider;
            _channelManager = channelManager;
            _customCommandHandler = customCommandHandler;
        }

        public async Task InitializeAsync()
        {
            _discordClient.MessageReceived += MessageReceivedAsync;
            _commandService.CommandExecuted += CommandExecutedAsync;

            await _commandService.AddModulesAsync(_assemblyProvider.GetCallingAssembly() ?? throw new InjhinuityException("Failed to load entry assembly."), _provider);
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            if (!(rawMessage is SocketUserMessage message) ||
                message.Source != MessageSource.User)
                return;

            var argPos = 0;
            if (!message.HasCharPrefix(_clientConfig.Discord.Prefix, ref argPos))
                return;

            var context = new SocketCommandContext((DiscordSocketClient)_discordClient, message);

            if (await _customCommandHandler.TryHandlingCustomCommand(context, message.Content[1..]))
                return;

            await _commandService.ExecuteAsync(context, argPos, _provider);
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!(result is InjhinuityCommandResult injhinuityResult))
                return;
            
            if (!(injhinuityResult.Embed is null))
            {
                await _channelManager.SendEmbedMessageAsync(context, injhinuityResult.Embed);
                return;
            }

            if (!(injhinuityResult.Message is null))
            {
                await _channelManager.SendMessageAsync(context, injhinuityResult.Message);
                return;
            }
        }
    }
}
