using System.Threading.Tasks;
using Injhinuity.Client.Core.Exceptions;
using Injhinuity.Client.Core.Resources;
using Injhinuity.Client.Discord.Embeds.Factories;
using Injhinuity.Client.Discord.Entities;
using Injhinuity.Client.Enums;
using Injhinuity.Client.Extensions;
using Injhinuity.Client.Model.Domain;
using Injhinuity.Client.Model.Domain.Responses;
using Injhinuity.Client.Services.Api;
using Injhinuity.Client.Services.Factories;
using Injhinuity.Client.Services.Requesters;

namespace Injhinuity.Client.Discord.Services
{
    public interface ICustomCommandHandlerService
    {
        Task<bool> TryHandlingCustomCommand(IInjhinuityCommandContext commandContext, string name);
    }

    public class CustomCommandHandlerService : ICustomCommandHandlerService
    {
        private readonly ICommandRequester _requester;
        private readonly ICommandBundleFactory _bundleFactory;
        private readonly IEmbedBuilderFactoryProvider _embedBuilderFactoryProvider;
        private readonly IApiReponseDeserializer _deserializer;
        private readonly ICommandExclusionService _commandExclusionService;

        public CustomCommandHandlerService(ICommandRequester requester, ICommandBundleFactory bundleFactory, IEmbedBuilderFactoryProvider embedBuilderFactoryProvider,
            IApiReponseDeserializer deserializer, ICommandExclusionService commandExclusionService)
        {
            _requester = requester;
            _bundleFactory = bundleFactory;
            _embedBuilderFactoryProvider = embedBuilderFactoryProvider;
            _deserializer = deserializer;
            _commandExclusionService = commandExclusionService;
        }

        public async Task<bool> TryHandlingCustomCommand(IInjhinuityCommandContext context, string message)
        {
            if (message.Split(' ').Length > 1 || _commandExclusionService.IsExcluded(message))
                return false;

            var bundle = _bundleFactory.Create(context.Guild.Id.ToString(), message);
            var apiResult = await _requester.ExecuteAsync(ApiAction.Get, bundle);

            if (apiResult.IsSuccessStatusCode)
            {
                var command = await _deserializer.DeserializeAndAdaptAsync<CommandResponse, Command>(apiResult);
                await context.Channel.SendMessageAsync(command?.Body ?? CommonResources.CommandNoBody);
            }
            else
            {
                var wrapper = await _deserializer.DeserializeAsync<ExceptionWrapper>(apiResult);
                var embedBuilder = _embedBuilderFactoryProvider.Command.CreateCustomFailure(wrapper);
                await context.Channel.SendEmbedMessageAsync(embedBuilder);
            }

            return true;
        }
    }
}
