using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Injhinuity.Client.Core.Validation.Enums;
using Injhinuity.Client.Core.Validation.Factories;
using Injhinuity.Client.Core.Validation.Validators;
using Injhinuity.Client.Discord.Builders;
using Injhinuity.Client.Discord.Embeds.Factories;
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
    public class CommandModule : BaseModule
    {
        private readonly ICommandRequester _requester;
        private readonly ICommandBundleFactory _bundleFactory;
        private readonly IEmbedBuilderFactoryProvider _embedBuilderFactoryProvider;
        private readonly ICommandValidator _validator;
        private readonly IReactionEmbedFactory _reactionEmbedFactory;
        private readonly IValidationResourceFactory _validationResourceFactory;

        public CommandModule(ICommandRequester requester, ICommandBundleFactory bundleFactory, ICommandResultBuilder resultBuilder,
            IEmbedBuilderFactoryProvider embedBuilderFactoryProvider, ICommandValidator validator, IApiReponseDeserializer deserializer, IReactionEmbedFactory reactionEmbedFactory,
            IInjhinuityCommandContextFactory commandContextFactory, IValidationResourceFactory validationResourceFactory) : base(commandContextFactory, deserializer, resultBuilder)
        {
            _requester = requester;
            _bundleFactory = bundleFactory;
            _embedBuilderFactoryProvider = embedBuilderFactoryProvider;
            _validator = validator;
            _reactionEmbedFactory = reactionEmbedFactory;
            _validationResourceFactory = validationResourceFactory;
        }

        [Command("create command")]
        public async Task<RuntimeResult> CreateAsync(string name, [Remainder] string body)
        {
            var resource = _validationResourceFactory.CreateCommand(name, body);
            var validationResult = _validator.Validate(resource);

            if (validationResult.ValidationCode != ValidationCode.Ok)
            {
                var validationEmbedBuilder = _embedBuilderFactoryProvider.Command.CreateFailure(validationResult);
                return EmbedResult(validationEmbedBuilder);
            }

            var bundle = _bundleFactory.Create(CustomContext.Guild.Id.ToString(), name, body);
            var apiResult = await _requester.ExecuteAsync(ApiAction.Post, bundle);

            var embedBuilder = apiResult.IsSuccessStatusCode
                ? _embedBuilderFactoryProvider.Command.CreateCreateSuccess(name, body)
                : _embedBuilderFactoryProvider.Command.CreateFailure(await GetExceptionWrapperAsync(apiResult));

            return EmbedResult(embedBuilder);
        }

        [Command("delete command")]
        public async Task<RuntimeResult> DeleteAsync(string name)
        {
            var bundle = _bundleFactory.Create(CustomContext.Guild.Id.ToString(), name);
            var apiResult = await _requester.ExecuteAsync(ApiAction.Delete, bundle);

            var embedBuilder = apiResult.IsSuccessStatusCode
                ? _embedBuilderFactoryProvider.Command.CreateDeleteSuccess(name)
                : _embedBuilderFactoryProvider.Command.CreateFailure(await GetExceptionWrapperAsync(apiResult));

            return EmbedResult(embedBuilder);
        }

        [Command("update command")]
        public async Task<RuntimeResult> UpdateAsync(string name, [Remainder] string body)
        {
            var bundle = _bundleFactory.Create(CustomContext.Guild.Id.ToString(), name, body);
            var apiResult = await _requester.ExecuteAsync(ApiAction.Put, bundle);

            var embedBuilder = apiResult.IsSuccessStatusCode
                ? _embedBuilderFactoryProvider.Command.CreateUpdateSuccess(name, body)
                : _embedBuilderFactoryProvider.Command.CreateFailure(await GetExceptionWrapperAsync(apiResult));

            return EmbedResult(embedBuilder);
        }

        [Command("get commands")]
        public async Task<RuntimeResult> GetAllAsync()
        {
            var bundle = _bundleFactory.Create(CustomContext.Guild.Id.ToString());
            var apiResult = await _requester.ExecuteAsync(ApiAction.GetAll, bundle);

            if (apiResult.IsSuccessStatusCode)
            {
                var embedBuilder = _embedBuilderFactoryProvider.Command.CreateGetAllSuccess();
                var commands = await DeserializeListAsync<CommandResponse, Command>(apiResult);
                var fieldList = commands?.Select(x => new InjhinuityEmbedField(x.Name, x.Body));

                var reactionEmbed = _reactionEmbedFactory.CreateListReactionEmbed(fieldList, embedBuilder);

                return ReactionEmbedResult(reactionEmbed);
            }
            else
            {
                var embedBuilder = _embedBuilderFactoryProvider.Command.CreateFailure(await GetExceptionWrapperAsync(apiResult));
                return EmbedResult(embedBuilder);
            }
        }
    }
}
