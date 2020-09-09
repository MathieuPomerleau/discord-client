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
        EmbedBuilder CreateAssignSuccess(string name);
        EmbedBuilder CreateUnassignSuccess(string name);
        EmbedBuilder CreateRoleNotFoundFailure(string name);
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
                .AddField(CommonResources.FieldTitleType, CommonResources.FieldValueTypeCreate, true)
                .AddField(CommonResources.FieldTitleName, name)
                .Build();

        public EmbedBuilder CreateDeleteSuccess(string name) =>
            CreateBaseSuccess()
                .AddField(CommonResources.FieldTitleType, CommonResources.FieldValueTypeDelete, true)
                .AddField(CommonResources.FieldTitleName, name)
                .Build();

        public EmbedBuilder CreateGetAllSuccess() =>
            _embedBuilder.Create()
                .WithTitle(RoleResources.TitlePlural)
                .WithThumbnailUrl(IconResources.List)
                .WithColor(Color.Orange)
                .WithTimestamp()
                .Build();

        public EmbedBuilder CreateAssignSuccess(string name) =>
            CreateBaseSuccess()
                .WithTitle(RoleResources.TitleAssign)
                .AddField(CommonResources.FieldTitleName, name)
                .Build();

        public EmbedBuilder CreateUnassignSuccess(string name) =>
            CreateBaseSuccess()
                .WithTitle(RoleResources.TitleUnassign)
                .AddField(CommonResources.FieldTitleName, name)
                .Build();

        public EmbedBuilder CreateRoleNotFoundFailure(string name) =>
            _embedBuilder.Create()
                .WithTitle(RoleResources.TitleRoleNotFound)
                .AddField(CommonResources.FieldTitleName, name)
                .WithThumbnailUrl(IconResources.Crossmark)
                .WithColor(Color.Red)
                .WithTimestamp()
                .Build();

        public EmbedBuilder CreateFailure(ExceptionWrapper wrapper) =>
            CreateBaseFailure(wrapper.Reason, wrapper.StatusCode)
                .Build();

        public EmbedBuilder CreateFailure(IValidationResult result) =>
            CreateBaseFailure(result.Message, result.ValidationCode)
                .Build();

        private IInjhinuityEmbedBuilder CreateBaseSuccess() =>
            _embedBuilder.Create()
                .WithTitle(RoleResources.Title)
                .AddField(CommonResources.FieldTitleResult, CommonResources.FieldValueResultSuccess, true)
                .WithThumbnailUrl(IconResources.Checkmark)
                .WithColor(Color.Green)
                .WithTimestamp();

        private IInjhinuityEmbedBuilder CreateBaseFailure(string? message, object errorCode) =>
            _embedBuilder.Create()
                .WithTitle(RoleResources.Title)
                .AddField(CommonResources.FieldTitleResult, CommonResources.FieldValueResultFailure, true)
                .AddField(CommonResources.FieldTitleErrorCode, errorCode, true)
                .AddField(CommonResources.FieldTitleReason, message ?? CommonResources.FieldValueReasonDefault)
                .WithThumbnailUrl(IconResources.Crossmark)
                .WithColor(Color.Red)
                .WithTimestamp();
    }
}
