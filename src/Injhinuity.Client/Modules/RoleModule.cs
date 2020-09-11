using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Injhinuity.Client.Core.Resources;
using Injhinuity.Client.Core.Validation.Enums;
using Injhinuity.Client.Core.Validation.Factories;
using Injhinuity.Client.Core.Validation.Validators;
using Injhinuity.Client.Discord.Builders;
using Injhinuity.Client.Discord.Embeds;
using Injhinuity.Client.Discord.Embeds.Factories;
using Injhinuity.Client.Discord.Factories;
using Injhinuity.Client.Enums;
using Injhinuity.Client.Model.Domain;
using Injhinuity.Client.Model.Domain.Responses;
using Injhinuity.Client.Services.Api;
using Injhinuity.Client.Services.Factories;
using Injhinuity.Client.Services.Requesters;

namespace Injhinuity.Client.Modules
{
    public class RoleModule : BaseModule
    {
        private readonly IRoleRequester _requester;
        private readonly IRoleBundleFactory _bundleFactory;
        private readonly IEmbedBuilderFactoryProvider _embedBuilderFactoryProvider;
        private readonly IRoleValidator _validator;
        private readonly IReactionEmbedFactory _reactionEmbedFactory;
        private readonly IValidationResourceFactory _validationResourceFactory;

        public RoleModule(IRoleRequester requester, IRoleBundleFactory bundleFactory, IEmbedBuilderFactoryProvider embedBuilderFactoryProvider, IRoleValidator validator, IReactionEmbedFactory reactionEmbedFactory,
            IValidationResourceFactory validationResourceFactory, IInjhinuityCommandContextFactory commandContextFactory, IApiReponseDeserializer deserializer, ICommandResultBuilder resultBuilder)
            : base(commandContextFactory, deserializer, resultBuilder)
        {
            _requester = requester;
            _bundleFactory = bundleFactory;
            _embedBuilderFactoryProvider = embedBuilderFactoryProvider;
            _validator = validator;
            _reactionEmbedFactory = reactionEmbedFactory;
            _validationResourceFactory = validationResourceFactory;
        }

        [Command("create role")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task<RuntimeResult> CreateAsync([Remainder] IRole role)
        {
            var resource = _validationResourceFactory.CreateRole(role.Name);
            var validationResult = _validator.Validate(resource);

            if (validationResult.ValidationCode != ValidationCode.Ok)
            {
                var validationEmbedBuilder = _embedBuilderFactoryProvider.Role.CreateFailure(validationResult);
                return EmbedResult(validationEmbedBuilder);
            }

            var bundle = _bundleFactory.Create(CustomContext.Guild.Id.ToString(), role);
            var apiResult = await _requester.ExecuteAsync(ApiAction.Post, bundle);

            var embedBuilder = apiResult.IsSuccessStatusCode
                ? _embedBuilderFactoryProvider.Role.CreateCreateSuccess(role.Name)
                : _embedBuilderFactoryProvider.Role.CreateFailure(await GetExceptionWrapperAsync(apiResult));

            return EmbedResult(embedBuilder);
        }

        [Command("delete role")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task<RuntimeResult> DeleteAsync([Remainder] IRole role)
        {
            var resource = _validationResourceFactory.CreateRole(role.Name);
            var validationResult = _validator.Validate(resource);

            if (validationResult.ValidationCode != ValidationCode.Ok)
            {
                var validationEmbedBuilder = _embedBuilderFactoryProvider.Role.CreateFailure(validationResult);
                return EmbedResult(validationEmbedBuilder);
            }

            var bundle = _bundleFactory.Create(CustomContext.Guild.Id.ToString(), role);
            var apiResult = await _requester.ExecuteAsync(ApiAction.Delete, bundle);

            var embedBuilder = apiResult.IsSuccessStatusCode
                ? _embedBuilderFactoryProvider.Role.CreateDeleteSuccess(role.Name)
                : _embedBuilderFactoryProvider.Role.CreateFailure(await GetExceptionWrapperAsync(apiResult));

            return EmbedResult(embedBuilder);
        }

        [Command("get roles")]
        public async Task<RuntimeResult> GetAllAsync()
        {
            var bundle = _bundleFactory.Create(CustomContext.Guild.Id.ToString());
            var apiResult = await _requester.ExecuteAsync(ApiAction.GetAll, bundle);

            if (apiResult.IsSuccessStatusCode)
            {
                var embedBuilder = _embedBuilderFactoryProvider.Role.CreateGetAllSuccess();
                var roles = await DeserializeListAsync<RoleResponse, Role>(apiResult);
                var fieldList = roles?.Select(x => new InjhinuityEmbedField(RoleResources.FieldTitleRole, x.Name));

                var reactionEmbed = _reactionEmbedFactory.CreateListReactionEmbed(fieldList, embedBuilder);

                return ReactionEmbedResult(reactionEmbed);
            }
            else
            {
                var embedBuilder = _embedBuilderFactoryProvider.Role.CreateFailure(await GetExceptionWrapperAsync(apiResult));
                return EmbedResult(embedBuilder);
            }
        }

        [Command("iam")]
        public async Task<RuntimeResult> AssignRoleAsync([Remainder] IRole role)
        {
            var bundle = _bundleFactory.Create(CustomContext.Guild.Id.ToString(), role);
            var apiResult = await _requester.ExecuteAsync(ApiAction.Get, bundle);

            if (apiResult.IsSuccessStatusCode)
            {
                await CustomContext.GuildUser.AddRoleAsync(role);
                var embedBuilder = _embedBuilderFactoryProvider.Role.CreateAssignSuccess(role.Name);

                return EmbedResult(embedBuilder);
            }
            else
            {
                var embedBuilder = _embedBuilderFactoryProvider.Role.CreateFailure(await GetExceptionWrapperAsync(apiResult));
                return EmbedResult(embedBuilder);
            }
        }

        [Command("iamnot")]
        public async Task<RuntimeResult> UnassignRoleAsync([Remainder] IRole role)
        {
            var bundle = _bundleFactory.Create(CustomContext.Guild.Id.ToString(), role);
            var apiResult = await _requester.ExecuteAsync(ApiAction.Get, bundle);

            if (apiResult.IsSuccessStatusCode)
            {
                await CustomContext.GuildUser.RemoveRoleAsync(role);
                var embedBuilder = _embedBuilderFactoryProvider.Role.CreateUnassignSuccess(role.Name);

                return EmbedResult(embedBuilder);
            }
            else
            {
                var embedBuilder = _embedBuilderFactoryProvider.Role.CreateFailure(await GetExceptionWrapperAsync(apiResult));
                return EmbedResult(embedBuilder);
            }
        }

        [Command("create role"), Alias("delete role", "iam", "iamnot")]
        public Task<RuntimeResult> RoleNotFoundAsync([Remainder] string remainder)
        {
            var embedBuilder = _embedBuilderFactoryProvider.Role.CreateRoleNotFoundFailure(remainder);
            return Task.FromResult(EmbedResult(embedBuilder));
        }
    }
}
