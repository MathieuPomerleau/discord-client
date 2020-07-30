using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Injhinuity.Client.Core.Exceptions;
using Injhinuity.Client.Discord.Builders;
using Injhinuity.Client.Enums;
using Injhinuity.Client.Model.Domain;
using Injhinuity.Client.Model.Domain.Responses;
using Injhinuity.Client.Services.Api;
using Injhinuity.Client.Services.EmbedFactories;
using Injhinuity.Client.Services.Factories;
using Injhinuity.Client.Services.Requesters;

namespace Injhinuity.Client.Modules
{
    public class CommandModule : ModuleBase<SocketCommandContext>
    {
        private readonly ICommandRequester _requester;
        private readonly ICommandPackageFactory _packageFactory;
        private readonly ICommandResultBuilder _resultBuilder;
        private readonly ICommandEmbedFactory _embedFactory;
        private readonly IApiReponseDeserializer _deserializer;

        public CommandModule(ICommandRequester requester, ICommandPackageFactory packageFactory, ICommandResultBuilder resultBuilder,
            ICommandEmbedFactory embedFactory, IApiReponseDeserializer deserializer)
        {
            _requester = requester;
            _packageFactory = packageFactory;
            _resultBuilder = resultBuilder;
            _embedFactory = embedFactory;
            _deserializer = deserializer;
        }

        [Command("create command")]
        public async Task<RuntimeResult> CreateAsync(string name, [Remainder] string body)
        {
            var package = _packageFactory.Create(Context.Guild.Id.ToString(), name, body);
            var apiResult = await _requester.ExecuteAsync(ApiAction.Post, package);

            var embed = apiResult.IsSuccessStatusCode
                ? _embedFactory.CreateCreateSuccessEmbed(name, body)
                : _embedFactory.CreateFailureEmbed(await GetExceptionWrapperAsync(apiResult));

            return EmbedResult(embed);
        }

        [Command("delete command")]
        public async Task<RuntimeResult> DeleteAsync(string name)
        {
            var package = _packageFactory.Create(Context.Guild.Id.ToString(), name);
            var apiResult = await _requester.ExecuteAsync(ApiAction.Delete, package);

            var embed = apiResult.IsSuccessStatusCode
                ? _embedFactory.CreateDeleteSuccessEmbed(name)
                : _embedFactory.CreateFailureEmbed(await GetExceptionWrapperAsync(apiResult));

            return EmbedResult(embed);
        }

        [Command("update command")]
        public async Task<RuntimeResult> UpdateAsync(string name, [Remainder] string body)
        {
            var package = _packageFactory.Create(Context.Guild.Id.ToString(), name, body);
            var apiResult = await _requester.ExecuteAsync(ApiAction.Put, package);

            var embed = apiResult.IsSuccessStatusCode
                ? _embedFactory.CreateUpdateSuccessEmbed(name, body)
                : _embedFactory.CreateFailureEmbed(await GetExceptionWrapperAsync(apiResult));

            return EmbedResult(embed);
        }

        [Command("get commands")]
        public async Task<RuntimeResult> GetAllAsync()
        {
            var package = _packageFactory.Create(Context.Guild.Id.ToString());
            var apiResult = await _requester.ExecuteAsync(ApiAction.GetAll, package);

            var embed = apiResult.IsSuccessStatusCode
                ? _embedFactory.CreateGetAllSuccessEmbed(await GetCommandListAsync(apiResult))
                : _embedFactory.CreateFailureEmbed(await GetExceptionWrapperAsync(apiResult));

            return EmbedResult(embed);
        }

        private Task<IEnumerable<Command>?> GetCommandListAsync(HttpResponseMessage apiResult) =>
            _deserializer.DeserializeAndAdaptEnumerableAsync<CommandResponse, Command>(apiResult);

        private Task<ExceptionWrapper> GetExceptionWrapperAsync(HttpResponseMessage apiResult) =>
            _deserializer.DeserializeAsync<ExceptionWrapper>(apiResult);

        private RuntimeResult EmbedResult(Embed embed) =>
            _resultBuilder.WithEmbed(embed).Build();
    }
}
