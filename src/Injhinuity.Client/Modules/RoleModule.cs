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
using Injhinuity.Client.Discord.Services;
using Injhinuity.Client.Enums;
using Injhinuity.Client.Model.Domain;
using Injhinuity.Client.Model.Domain.Requests;
using Injhinuity.Client.Model.Domain.Responses;
using Injhinuity.Client.Services.Api;
using Injhinuity.Client.Services.Factories;
using Injhinuity.Client.Services.Mappers;
using Injhinuity.Client.Services.Requesters;

namespace Injhinuity.Client.Modules
{
    public class RoleModule : BaseModule
    {
        private readonly IRoleRequester _roleRequester;
        private readonly IRoleBundleFactory _roleBundleFactory;
        private readonly IRoleValidator _validator;
        private readonly IGuildRequester _guildRequester;
        private readonly IGuildBundleFactory _guildBundleFactory;
        private readonly IReactionEmbedFactory _reactionEmbedFactory;
        private readonly IValidationResourceFactory _validationResourceFactory;
        private readonly IReactionRoleEmbedService _reactionRoleEmbedService;

        public RoleModule(IRoleRequester roleRequester, IRoleBundleFactory roleBundleFactory, IEmbedBuilderFactoryProvider embedBuilderFactoryProvider,
            IRoleValidator validator, IGuildRequester guildRequester, IGuildBundleFactory guildBundleFactory, IReactionEmbedFactory reactionEmbedFactory,
            IValidationResourceFactory validationResourceFactory, IInjhinuityCommandContextFactory commandContextFactory, IApiReponseDeserializer deserializer,
            ICommandResultBuilder resultBuilder, IReactionRoleEmbedService reactionRoleEmbedService, IInjhinuityMapper mapper)
            : base(commandContextFactory, deserializer, resultBuilder, mapper, embedBuilderFactoryProvider)
        {
            _roleRequester = roleRequester;
            _roleBundleFactory = roleBundleFactory;
            _validator = validator;
            _guildRequester = guildRequester;
            _guildBundleFactory = guildBundleFactory;
            _reactionEmbedFactory = reactionEmbedFactory;
            _validationResourceFactory = validationResourceFactory;
            _reactionRoleEmbedService = reactionRoleEmbedService;
        }

        [Command("create role")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task<RuntimeResult> CreateRoleAsync(string emote, [Remainder] IRole role)
        {
            var resource = _validationResourceFactory.CreateRole(role.Name, emote);
            var validationResult = _validator.Validate(resource);

            if (validationResult.ValidationCode != ValidationCode.Ok)
                return EmbedResult(EmbedBuilderFactoryProvider.Role.CreateFailure(validationResult));

            var roleBundle = _roleBundleFactory.Create(CustomContext.Guild.Id.ToString(), role.Id.ToString(), role.Name, emote);
            var roleResponse = await _roleRequester.ExecuteAsync(ApiAction.Post, roleBundle);

            var embedBuilder = roleResponse.IsSuccessStatusCode
                ? EmbedBuilderFactoryProvider.Role.CreateCreateSuccess(role.Name)
                : EmbedBuilderFactoryProvider.Role.CreateFailure(await GetExceptionWrapperAsync(roleResponse));

            if (roleResponse.IsSuccessStatusCode)
                TryUpdatingRoleReactionEmbed();

            return EmbedResult(embedBuilder);
        }

        [Command("delete role")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task<RuntimeResult> DeleteRoleAsync([Remainder] IRole role)
        {
            var roleBundle = _roleBundleFactory.Create(CustomContext.Guild.Id.ToString(), role.Id.ToString(), role.Name);
            var roleResponse = await _roleRequester.ExecuteAsync(ApiAction.Delete, roleBundle);

            var embedBuilder = roleResponse.IsSuccessStatusCode
                ? EmbedBuilderFactoryProvider.Role.CreateDeleteSuccess(role.Name)
                : EmbedBuilderFactoryProvider.Role.CreateFailure(await GetExceptionWrapperAsync(roleResponse));

            if (roleResponse.IsSuccessStatusCode)
                TryUpdatingRoleReactionEmbed();

            return EmbedResult(embedBuilder);
        }

        [Command("get roles")]
        public async Task<RuntimeResult> GetRolesAsync()
        {
            var roleBundle = _roleBundleFactory.Create(CustomContext.Guild.Id.ToString());
            var roleResponse = await _roleRequester.ExecuteAsync(ApiAction.GetAll, roleBundle);

            if (roleResponse.IsSuccessStatusCode)
            {
                var roles = await StrictDeserializeListAsync<RoleResponse, Role>(roleResponse);
                var fieldList = roles.Select(x => new InjhinuityEmbedField(RoleResources.FieldTitleRole, x.Name));

                var embedBuilder = EmbedBuilderFactoryProvider.Role.CreateGetAllSuccess();
                var reactionEmbed = _reactionEmbedFactory.CreatePageReactionEmbed(embedBuilder, fieldList);
                return ReactionEmbedResult(reactionEmbed);
            }
            else
            {
                var embedBuilder = EmbedBuilderFactoryProvider.Role.CreateFailure(await GetExceptionWrapperAsync(roleResponse));
                return EmbedResult(embedBuilder);
            }
        }

        [Command("setup roles")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task<RuntimeResult> SetupRolesAsync()
        {
            var guildBundle = _guildBundleFactory.Create(CustomContext.Guild.Id.ToString());
            var guildResponse = await _guildRequester.ExecuteAsync(ApiAction.Get, guildBundle);
            if (!guildResponse.IsSuccessStatusCode)
                return EmbedResult(EmbedBuilderFactoryProvider.Role.CreateFailure(await GetExceptionWrapperAsync(guildResponse)));

            var roleSettings = (await StrictDeserializeAsync<GuildResponse, Guild>(guildResponse)).RoleSettings;
            if (roleSettings.IsReactionRoleSetup())
                return EmbedResult(EmbedBuilderFactoryProvider.Role.CreateRolesAlreadySetupFailure(roleSettings.ReactionRoleChannelId));

            var roleBundle = _roleBundleFactory.Create(CustomContext.Guild.Id.ToString());
            var roleResponse = await _roleRequester.ExecuteAsync(ApiAction.GetAll, roleBundle);
            if (!roleResponse.IsSuccessStatusCode)
                return EmbedResult(EmbedBuilderFactoryProvider.Role.CreateFailure(await GetExceptionWrapperAsync(roleResponse)));

            var domainRoles = await StrictDeserializeListAsync<RoleResponse, Role>(roleResponse);
            var messageId = await _reactionRoleEmbedService.InitializeAsync(CustomContext, domainRoles);

            var newRoleSettingsRequest = StrictMap<RoleGuildSettings, RoleGuildSettingsRequest>(roleSettings with
            {
                ReactionRoleChannelId = CustomContext.Channel.Id.ToString(),
                ReactionRoleMessageId = messageId.ToString()
            });

            var updateGuildBundle = _guildBundleFactory.Create(CustomContext.Guild.Id.ToString(), newRoleSettingsRequest);
            var updateGuildResponse = await _guildRequester.ExecuteAsync(ApiAction.Put, updateGuildBundle);
            if (!updateGuildResponse.IsSuccessStatusCode)
            {
                await _reactionRoleEmbedService.ResetAsync();
                return EmbedResult(EmbedBuilderFactoryProvider.Role.CreateFailure(await GetExceptionWrapperAsync(updateGuildResponse)));
            }

            return EmbedResult(EmbedBuilderFactoryProvider.Role.CreateRolesSetupSuccess());
        }

        [Command("reset roles")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task<RuntimeResult> ResetRoleAsync()
        {
            var guildBundle = _guildBundleFactory.Create(CustomContext.Guild.Id.ToString());
            var guildResponse = await _guildRequester.ExecuteAsync(ApiAction.Get, guildBundle);
            if (!guildResponse.IsSuccessStatusCode)
                return EmbedResult(EmbedBuilderFactoryProvider.Role.CreateFailure(await GetExceptionWrapperAsync(guildResponse)));

            var roleSettings = (await StrictDeserializeAsync<GuildResponse, Guild>(guildResponse)).RoleSettings;
            if (!roleSettings.IsReactionRoleSetup())
                return EmbedResult(EmbedBuilderFactoryProvider.Role.CreateRolesNotSetupFailure());

            await _reactionRoleEmbedService.ResetAsync();

            var newRoleSettingsRequest = StrictMap<RoleGuildSettings, RoleGuildSettingsRequest>(roleSettings with
            {
                ReactionRoleChannelId = string.Empty,
                ReactionRoleMessageId = string.Empty
            });

            var updateGuildBundle = _guildBundleFactory.Create(CustomContext.Guild.Id.ToString(), newRoleSettingsRequest);
            var updateGuildResponse = await _guildRequester.ExecuteAsync(ApiAction.Put, updateGuildBundle);

            var updateEmbedBuilder = updateGuildResponse.IsSuccessStatusCode
                ? EmbedBuilderFactoryProvider.Role.CreateRolesResetSuccess()
                : EmbedBuilderFactoryProvider.Role.CreateFailure(await GetExceptionWrapperAsync(updateGuildResponse));

            return EmbedResult(updateEmbedBuilder);
        }

        [Command("create role"), Alias("delete role")]
        public Task<RuntimeResult> RoleNotFoundAsync([Remainder] string remainder)
        {
            var embedBuilder = EmbedBuilderFactoryProvider.Role.CreateRoleNotFoundFailure(remainder);
            return Task.FromResult(EmbedResult(embedBuilder));
        }

        public async void TryUpdatingRoleReactionEmbed()
        {
            var guildBundle = _guildBundleFactory.Create(CustomContext.Guild.Id.ToString());
            var guildResponse = await _guildRequester.ExecuteAsync(ApiAction.Get, guildBundle);
            if (!guildResponse.IsSuccessStatusCode)
                return;

            var roleSettings = (await StrictDeserializeAsync<GuildResponse, Guild>(guildResponse)).RoleSettings;
            if (!roleSettings.IsReactionRoleSetup())
                return;

            var roleChannel = await CustomContext.Guild.GetTextChannelAsync(ulong.Parse(roleSettings.ReactionRoleChannelId));
            if (roleChannel is not null && await roleChannel.GetMessageAsync(ulong.Parse(roleSettings.ReactionRoleMessageId)) is IUserMessage message)
            {
                var roleBundle = _roleBundleFactory.Create(CustomContext.Guild.Id.ToString());
                var roleResponse = await _roleRequester.ExecuteAsync(ApiAction.GetAll, roleBundle);
                if (!roleResponse.IsSuccessStatusCode)
                    return;

                var domainRoles = await StrictDeserializeListAsync<RoleResponse, Role>(roleResponse);
                await _reactionRoleEmbedService.InitializeFromMessageAsync(CustomContext.Guild, message, domainRoles);
            }
        }
    }
}
