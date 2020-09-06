using Discord;
using Injhinuity.Client.Core.Exceptions;
using Injhinuity.Client.Core.Resources;
using Injhinuity.Client.Core.Validation.Entities;
using Injhinuity.Client.Discord.Builders;

namespace Injhinuity.Client.Discord.Embeds.Factories
{
    public interface IRoleEmbedBuilderFactory
    {
        EmbedBuilder CreateCreateSuccessEmbedBuilder(string name);
        EmbedBuilder CreateDeleteSuccessEmbedBuilder(string name);
        EmbedBuilder CreateGetAllSuccessEmbedBuilder();
        EmbedBuilder CreateFailureEmbedBuilder(ExceptionWrapper wrapper);
        EmbedBuilder CreateFailureEmbedBuilder(IValidationResult result);
    }

    public class RoleEmbedBuilderFactory : IRoleEmbedBuilderFactory
    {
        private readonly IInjhinuityEmbedBuilder _embedBuilder;

        public RoleEmbedBuilderFactory(IInjhinuityEmbedBuilder embedBuilder)
        {
            _embedBuilder = embedBuilder;
        }

        public EmbedBuilder CreateCreateSuccessEmbedBuilder(string name) =>
            CreateBaseSuccessEmbed()
                .AddField(CommonResources.FieldTitleType, CommonResources.FieldValueTypeCreate, true)
                .AddField(CommonResources.FieldTitleName, name)
                .Build();

        public EmbedBuilder CreateDeleteSuccessEmbedBuilder(string name) =>
            CreateBaseSuccessEmbed()
                .AddField(CommonResources.FieldTitleType, CommonResources.FieldValueTypeDelete, true)
                .AddField(CommonResources.FieldTitleName, name)
                .Build();

        public EmbedBuilder CreateGetAllSuccessEmbedBuilder()
        {
            return _embedBuilder.Create()
                .WithTitle(RoleResources.TitlePlural)
                .WithThumbnailUrl(IconResources.List)
                .WithColor(Color.Orange)
                .WithTimestamp()
                .Build();
        }

        public EmbedBuilder CreateFailureEmbedBuilder(ExceptionWrapper wrapper) =>
            CreateBaseFailureEmbed(wrapper.Reason, wrapper.StatusCode)
                .Build();

        public EmbedBuilder CreateFailureEmbedBuilder(IValidationResult result) =>
            CreateBaseFailureEmbed(result.Message, result.ValidationCode)
                .Build();

        private IInjhinuityEmbedBuilder CreateBaseSuccessEmbed() =>
            _embedBuilder.Create()
                .WithTitle(RoleResources.Title)
                .AddField(CommonResources.FieldTitleResult, CommonResources.FieldValueResultSuccess, true)
                .WithThumbnailUrl(IconResources.Checkmark)
                .WithColor(Color.Green)
                .WithTimestamp();

        private IInjhinuityEmbedBuilder CreateBaseFailureEmbed(string? message, object errorCode) =>
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
