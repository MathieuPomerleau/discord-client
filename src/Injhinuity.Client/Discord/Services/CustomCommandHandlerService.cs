using System.Net;
using System.Threading.Tasks;
using Discord.Commands;
using Injhinuity.Client.Core.Exceptions;
using Injhinuity.Client.Discord.Managers;
using Injhinuity.Client.Enums;
using Injhinuity.Client.Model.Domain;
using Injhinuity.Client.Model.Domain.Responses;
using Injhinuity.Client.Services.Api;
using Injhinuity.Client.Services.EmbedFactories;
using Injhinuity.Client.Services.Factories;
using Injhinuity.Client.Services.Requesters;

namespace Injhinuity.Client.Discord.Services
{
    public interface ICustomCommandHandlerService
    {
        Task<bool> TryHandlingCustomCommand(ICommandContext commandContext, string name);
    }

    public class CustomCommandHandlerService : ICustomCommandHandlerService
    {
        private readonly ICommandRequester _requester;
        private readonly ICommandPackageFactory _packageFactory;
        private readonly ICommandEmbedFactory _embedFactory;
        private readonly IApiReponseDeserializer _deserializer;
        private readonly IChannelManager _channelManager;

        public CustomCommandHandlerService(ICommandRequester requester, ICommandPackageFactory packageFactory, ICommandEmbedFactory embedFactory,
            IApiReponseDeserializer deserializer, IChannelManager channelManager)
        {
            _requester = requester;
            _packageFactory = packageFactory;
            _embedFactory = embedFactory;
            _deserializer = deserializer;
            _channelManager = channelManager;
        }

        public async Task<bool> TryHandlingCustomCommand(ICommandContext context, string message)
        {
            if (message.Split(' ').Length > 1)
                return false;

            var package = _packageFactory.Create(context.Guild.Id.ToString(), message);
            var apiResult = await _requester.ExecuteAsync(ApiAction.Get, package);

            if (apiResult.IsSuccessStatusCode)
            {
                var command = await _deserializer.DeserializeAndAdaptAsync<CommandResponse, Command>(apiResult);
                await _channelManager.SendMessageAsync(context, command?.Body ?? "Command had no content");
            }
            else
            {
                var wrapper = await _deserializer.DeserializeAsync<ExceptionWrapper>(apiResult);
                if (wrapper.StatusCode != HttpStatusCode.NotFound)
                    return false;

                var embed = _embedFactory.CreateCustomFailureEmbed(wrapper);
                await _channelManager.SendEmbedMessageAsync(context, embed);
            }

            return true;
        }
    }
}
