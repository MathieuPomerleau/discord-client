﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Injhinuity.Client.Core.Exceptions;
using Injhinuity.Client.Core.Validation.Enums;
using Injhinuity.Client.Core.Validation.Factories;
using Injhinuity.Client.Core.Validation.Validators;
using Injhinuity.Client.Discord.Builders;
using Injhinuity.Client.Discord.Embeds;
using Injhinuity.Client.Discord.Entities;
using Injhinuity.Client.Discord.Factories;
using Injhinuity.Client.Enums;
using Injhinuity.Client.Model.Domain;
using Injhinuity.Client.Model.Domain.Responses;
using Injhinuity.Client.Services.Api;
using Injhinuity.Client.Services.Factories;
using Injhinuity.Client.Services.Requesters;
using InjhinuityEmbedField = Injhinuity.Client.Discord.Embeds.InjhinuityEmbedField;

namespace Injhinuity.Client.Modules
{
    public class CommandModule : ModuleBase<SocketCommandContext>
    {
        private readonly ICommandRequester _requester;
        private readonly ICommandBundleFactory _bundleFactory;
        private readonly ICommandResultBuilder _resultBuilder;
        private readonly ICommandEmbedFactory _embedFactory;
        private readonly ICommandValidator _commandValidator;
        private readonly IApiReponseDeserializer _deserializer;
        private readonly IReactionEmbedFactory _reactionEmbedFactory;
        private readonly IInjhinuityCommandContextFactory _commandContextFactory;
        private readonly IValidationResourceFactory _validationResourceFactory;

        private IInjhinuityCommandContext CommandContext => _commandContextFactory.Create(Context);

        public CommandModule(ICommandRequester requester, ICommandBundleFactory bundleFactory, ICommandResultBuilder resultBuilder,
            ICommandEmbedFactory embedFactory, ICommandValidator commandValidator, IApiReponseDeserializer deserializer, IReactionEmbedFactory reactionEmbedFactory,
            IInjhinuityCommandContextFactory commandContextFactory, IValidationResourceFactory validationResourceFactory)
        {
            _requester = requester;
            _bundleFactory = bundleFactory;
            _resultBuilder = resultBuilder;
            _embedFactory = embedFactory;
            _commandValidator = commandValidator;
            _deserializer = deserializer;
            _reactionEmbedFactory = reactionEmbedFactory;
            _commandContextFactory = commandContextFactory;
            _validationResourceFactory = validationResourceFactory;
        }

        [Command("create command")]
        public async Task<RuntimeResult> CreateAsync(string name, [Remainder] string body)
        {
            var resource = _validationResourceFactory.CreateCommand(name, body);
            var validationResult = _commandValidator.Validate(resource);

            if (validationResult.ValidationCode != ValidationCode.Ok)
            {
                var validationEmbedBuilder = _embedFactory.CreateFailureEmbedBuilder(validationResult);
                return EmbedResult(validationEmbedBuilder);
            }

            var bundle = _bundleFactory.Create(CommandContext.Guild.Id.ToString(), name, body);
            var apiResult = await _requester.ExecuteAsync(ApiAction.Post, bundle);

            var embedBuilder = apiResult.IsSuccessStatusCode
                ? _embedFactory.CreateCreateSuccessEmbedBuilder(name, body)
                : _embedFactory.CreateFailureEmbedBuilder(await GetExceptionWrapperAsync(apiResult));

            return EmbedResult(embedBuilder);
        }

        [Command("delete command")]
        public async Task<RuntimeResult> DeleteAsync(string name)
        {
            var bundle = _bundleFactory.Create(CommandContext.Guild.Id.ToString(), name);
            var apiResult = await _requester.ExecuteAsync(ApiAction.Delete, bundle);

            var embedBuilder = apiResult.IsSuccessStatusCode
                ? _embedFactory.CreateDeleteSuccessEmbedBuilder(name)
                : _embedFactory.CreateFailureEmbedBuilder(await GetExceptionWrapperAsync(apiResult));

            return EmbedResult(embedBuilder);
        }

        [Command("update command")]
        public async Task<RuntimeResult> UpdateAsync(string name, [Remainder] string body)
        {
            var bundle = _bundleFactory.Create(CommandContext.Guild.Id.ToString(), name, body);
            var apiResult = await _requester.ExecuteAsync(ApiAction.Put, bundle);

            var embedBuilder = apiResult.IsSuccessStatusCode
                ? _embedFactory.CreateUpdateSuccessEmbedBuilder(name, body)
                : _embedFactory.CreateFailureEmbedBuilder(await GetExceptionWrapperAsync(apiResult));

            return EmbedResult(embedBuilder);
        }

        [Command("get commands")]
        public async Task<RuntimeResult> GetAllAsync()
        {
            var bundle = _bundleFactory.Create(CommandContext.Guild.Id.ToString());
            var apiResult = await _requester.ExecuteAsync(ApiAction.GetAll, bundle);

            if (apiResult.IsSuccessStatusCode)
            {
                var embedBuilder = _embedFactory.CreateGetAllSuccessEmbedBuilder();
                var commands = await GetCommandListAsync(apiResult);
                var fieldList = commands?.Select(x => new InjhinuityEmbedField(x.Name, x.Body));

                var reactionEmbed = _reactionEmbedFactory.CreateListReactionEmbed(fieldList, embedBuilder);

                return ReactionEmbedResult(reactionEmbed);
            }
            else
            {
                var embedBuilder = _embedFactory.CreateFailureEmbedBuilder(await GetExceptionWrapperAsync(apiResult));
                return EmbedResult(embedBuilder);
            }
        }

        private Task<IEnumerable<Command>?> GetCommandListAsync(HttpResponseMessage apiResult) =>
            _deserializer.DeserializeAndAdaptEnumerableAsync<CommandResponse, Command>(apiResult);

        private Task<ExceptionWrapper> GetExceptionWrapperAsync(HttpResponseMessage apiResult) =>
            _deserializer.DeserializeAsync<ExceptionWrapper>(apiResult);

        private RuntimeResult EmbedResult(EmbedBuilder embedBuilder) =>
            _resultBuilder.Create()
                .WithEmbedBuilder(embedBuilder)
                .Build();

        private RuntimeResult ReactionEmbedResult(IReactionEmbed embed) =>
            _resultBuilder.Create()
                .WithReactionEmbed(embed)
                .Build();
    }
}
