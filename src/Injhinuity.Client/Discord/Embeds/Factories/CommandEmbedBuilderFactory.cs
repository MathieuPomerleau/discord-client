using Discord;
using Injhinuity.Client.Core.Exceptions;
using Injhinuity.Client.Core.Resources;
using Injhinuity.Client.Core.Validation.Entities;
using Injhinuity.Client.Discord.Builders;

namespace Injhinuity.Client.Discord.Embeds.Factories
{
    public interface ICommandEmbedBuilderFactory
    {
        EmbedBuilder CreateCreateSuccessEmbedBuilder(string name, string body);
        EmbedBuilder CreateDeleteSuccessEmbedBuilder(string name);
        EmbedBuilder CreateUpdateSuccessEmbedBuilder(string name, string body);
        EmbedBuilder CreateGetAllSuccessEmbedBuilder();
        EmbedBuilder CreateFailureEmbedBuilder(ExceptionWrapper wrapper);
        EmbedBuilder CreateFailureEmbedBuilder(IValidationResult result);
        EmbedBuilder CreateCustomFailureEmbedBuilder(ExceptionWrapper wrapper);
    }

    public class CommandEmbedBuilderFactory : ICommandEmbedBuilderFactory
    {
        private readonly IInjhinuityEmbedBuilder _embedBuilder;

        public CommandEmbedBuilderFactory(IInjhinuityEmbedBuilder embedBuilder)
        {
            _embedBuilder = embedBuilder;
        }

        public EmbedBuilder CreateCreateSuccessEmbedBuilder(string name, string body) =>
            CreateBaseSuccessEmbed()
                .AddField(CommonResources.FieldTitleType, CommonResources.FieldValueTypeCreate, true)
                .AddField(CommonResources.FieldTitleName, name)
                .AddField(CommonResources.FieldTitleContent, body, true)
                .Build();

        public EmbedBuilder CreateDeleteSuccessEmbedBuilder(string name) =>
            CreateBaseSuccessEmbed()
                .AddField(CommonResources.FieldTitleType, CommonResources.FieldValueTypeDelete, true)
                .AddField(CommonResources.FieldTitleName, name)
                .Build();

        public EmbedBuilder CreateUpdateSuccessEmbedBuilder(string name, string body) =>
            CreateBaseSuccessEmbed()
                .AddField(CommonResources.FieldTitleType, CommonResources.FieldValueTypeUpdate, true)
                .AddField(CommonResources.FieldTitleName, name)
                .AddField(CommonResources.FieldTitleContent, body, true)
                .Build();

        public EmbedBuilder CreateGetAllSuccessEmbedBuilder()
        {
            return _embedBuilder.Create()
                .WithTitle(CommandResources.TitlePlural)
                .WithThumbnailUrl(IconResources.List)
                .WithColor(Color.Orange)
                .WithTimestamp()
                .Build();
        }

        public EmbedBuilder CreateCustomFailureEmbedBuilder(ExceptionWrapper wrapper) =>
            CreateBaseFailureEmbed(wrapper.Reason, wrapper.StatusCode)
                .WithTitle(CommandResources.TitleCustom)
                .Build();

        public EmbedBuilder CreateFailureEmbedBuilder(ExceptionWrapper wrapper) =>
            CreateBaseFailureEmbed(wrapper.Reason, wrapper.StatusCode)
                .Build();

        public EmbedBuilder CreateFailureEmbedBuilder(IValidationResult result) =>
            CreateBaseFailureEmbed(result.Message, result.ValidationCode)
                .Build();

        private IInjhinuityEmbedBuilder CreateBaseSuccessEmbed() =>
            _embedBuilder.Create()
                .WithTitle(CommandResources.Title)
                .AddField(CommonResources.FieldTitleResult, CommonResources.FieldValueResultSuccess, true)
                .WithThumbnailUrl(IconResources.Checkmark)
                .WithColor(Color.Green)
                .WithTimestamp();

        private IInjhinuityEmbedBuilder CreateBaseFailureEmbed(string? message, object errorCode) =>
            _embedBuilder.Create()
                .WithTitle(CommandResources.Title)
                .AddField(CommonResources.FieldTitleResult, CommonResources.FieldValueResultFailure, true)
                .AddField(CommonResources.FieldTitleErrorCode, errorCode, true)
                .AddField(CommonResources.FieldTitleReason, message ?? CommonResources.FieldValueReasonDefault)
                .WithThumbnailUrl(IconResources.Crossmark)
                .WithColor(Color.Red)
                .WithTimestamp();
    }
}
