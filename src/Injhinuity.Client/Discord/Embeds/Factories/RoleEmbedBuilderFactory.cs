using Discord;
using Injhinuity.Client.Core.Exceptions;
using Injhinuity.Client.Core.Resources;
using Injhinuity.Client.Core.Validation.Entities;
using Injhinuity.Client.Discord.Builders;

namespace Injhinuity.Client.Discord.Embeds.Factories
{
    public interface IRoleEmbedBuilderFactory
    {
        EmbedBuilder CreateCreateSuccess(string name);
        EmbedBuilder CreateDeleteSuccess(string name);
        EmbedBuilder CreateGetAllSuccess();
        EmbedBuilder CreateAssignRoleSuccess(string username, string rolename);
        EmbedBuilder CreateUnassignRoleSuccess(string username, string rolename);
        EmbedBuilder CreateRolesSetupSuccess();
        EmbedBuilder CreateRolesResetSuccess();
        EmbedBuilder CreateRoleNotFoundFailure(string roleName);
        EmbedBuilder CreateRolesAlreadySetupFailure(string channelId);
        EmbedBuilder CreateReactionRole();
        EmbedBuilder CreateRolesNotSetupFailure();
        EmbedBuilder CreateFailure(ExceptionWrapper wrapper);
        EmbedBuilder CreateFailure(IValidationResult result);
    }

    public class RoleEmbedBuilderFactory : IRoleEmbedBuilderFactory
    {
        private readonly IInjhinuityEmbedBuilder _embedBuilder;

        public RoleEmbedBuilderFactory(IInjhinuityEmbedBuilder embedBuilder)
        {
            _embedBuilder = embedBuilder;
        }

        public EmbedBuilder CreateCreateSuccess(string name) =>
            CreateBaseSuccess()
                .AddField(CommonResources.FieldTitleName, name)
                .Build();

        public EmbedBuilder CreateDeleteSuccess(string name) =>
            CreateBaseSuccess()
                .AddField(CommonResources.FieldTitleName, name)
                .Build();

        public EmbedBuilder CreateGetAllSuccess() =>
            _embedBuilder.Create()
                .WithTitle(RoleResources.TitlePlural)
                .WithThumbnailUrl(IconResources.List)
                .WithColor(Color.Orange)
                .Build();

        public EmbedBuilder CreateAssignRoleSuccess(string username, string rolename) =>
            CreateBaseSuccess()
                .WithTitle(RoleResources.TitleAssign)
                .AddField(CommonResources.FieldTitleUser, username, true)
                .AddField(CommonResources.FieldTitleRole, rolename, true)
                .Build();

        public EmbedBuilder CreateUnassignRoleSuccess(string username, string rolename) =>
            CreateBaseSuccess()
                .WithTitle(RoleResources.TitleUnassign)
                .AddField(CommonResources.FieldTitleUser, username, true)
                .AddField(CommonResources.FieldTitleRole, rolename, true)
                .Build();

        public EmbedBuilder CreateRolesSetupSuccess() =>
            CreateBaseSuccess()
                .WithTitle(RoleResources.TitleRolesSetup)
                .Build();

        public EmbedBuilder CreateRolesResetSuccess() =>
            CreateBaseSuccess()
                .WithTitle(RoleResources.TitleRolesReset)
                .Build();

        public EmbedBuilder CreateReactionRole() =>
            _embedBuilder.Create()
                .WithTitle(RoleResources.TitlePlural)
                .WithThumbnailUrl(IconResources.List)
                .WithColor(Color.Purple)
                .Build();

        public EmbedBuilder CreateRoleNotFoundFailure(string roleName) =>
            CreateBaseFailure()
                .WithTitle(RoleResources.TitleRoleNotFound)
                .AddField(CommonResources.FieldTitleName, roleName)
                .Build();

        public EmbedBuilder CreateRolesAlreadySetupFailure(string channelId) =>
            CreateBaseFailure()
                .WithTitle(RoleResources.TitleRolesAlreadySetup)
                .AddField(CommonResources.FieldTitleChannelId, channelId)
                .Build();

        public EmbedBuilder CreateRolesNotSetupFailure() =>
            CreateBaseFailure()
                .WithTitle(RoleResources.TitleRolesNotSetup)
                .Build();

        public EmbedBuilder CreateFailure(ExceptionWrapper wrapper) =>
            CreateBaseFailure()
                .AddField(CommonResources.FieldTitleErrorCode, wrapper.StatusCode, true)
                .AddField(CommonResources.FieldTitleReason, wrapper.Reason ?? CommonResources.FieldValueReasonDefault)
                .Build();

        public EmbedBuilder CreateFailure(IValidationResult result) =>
            CreateBaseFailure()
                .AddField(CommonResources.FieldTitleErrorCode, result.ValidationCode, true)
                .AddField(CommonResources.FieldTitleReason, result.Message ?? CommonResources.FieldValueReasonDefault)
                .Build();

        private IInjhinuityEmbedBuilder CreateBaseSuccess() =>
            _embedBuilder.Create()
                .WithTitle(RoleResources.Title)
                .WithThumbnailUrl(IconResources.Checkmark)
                .WithColor(Color.Green);

        private IInjhinuityEmbedBuilder CreateBaseFailure() =>
            _embedBuilder.Create()
                .WithTitle(RoleResources.Title)
                .WithThumbnailUrl(IconResources.Crossmark)
                .WithColor(Color.Red);
    }
}
