using Discord;
using Injhinuity.Client.Core.Exceptions;
using Injhinuity.Client.Core.Resources;
using Injhinuity.Client.Core.Validation.Entities;
using Injhinuity.Client.Discord.Builders;

namespace Injhinuity.Client.Discord.Factories
{
    public interface ICommandEmbedFactory
    {
        EmbedBuilder CreateCreateSuccessEmbedBuilder(string name, string body);
        EmbedBuilder CreateDeleteSuccessEmbedBuilder(string name);
        EmbedBuilder CreateUpdateSuccessEmbedBuilder(string name, string body);
        EmbedBuilder CreateGetAllSuccessEmbedBuilder();
        EmbedBuilder CreateFailureEmbedBuilder(ExceptionWrapper wrapper);
        EmbedBuilder CreateFailureEmbedBuilder(IValidationResult result);
        EmbedBuilder CreateCustomFailureEmbedBuilder(ExceptionWrapper wrapper);
    }

    public class CommandEmbedFactory : ICommandEmbedFactory
    {
        private readonly IInjhinuityEmbedBuilder _embedBuilder;

        public CommandEmbedFactory(IInjhinuityEmbedBuilder embedBuilder)
        {
            _embedBuilder = embedBuilder;
        }

        public EmbedBuilder CreateCreateSuccessEmbedBuilder(string name, string body) =>
            CreateBaseSuccessEmbed()
                .AddField(CommandResources.FieldTitleType, CommandResources.FieldValueTypeCreate, true)
                .AddField(CommandResources.FieldTitleName, name)
                .AddField(CommandResources.FieldTitleContent, body, true)
                .Build();

        public EmbedBuilder CreateDeleteSuccessEmbedBuilder(string name) =>
            CreateBaseSuccessEmbed()
                .AddField(CommandResources.FieldTitleType, CommandResources.FieldValueTypeDelete, true)
                .AddField(CommandResources.FieldTitleName, name)
                .Build();

        public EmbedBuilder CreateUpdateSuccessEmbedBuilder(string name, string body) =>
            CreateBaseSuccessEmbed()
                .AddField(CommandResources.FieldTitleType, CommandResources.FieldValueTypeUpdate, true)
                .AddField(CommandResources.FieldTitleName, name)
                .AddField(CommandResources.FieldTitleContent, body, true)
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
                .AddField(CommandResources.FieldTitleResult, CommandResources.FieldValueResultSuccess, true)
                .WithThumbnailUrl(IconResources.Checkmark)
                .WithColor(Color.Green)
                .WithTimestamp();

        private IInjhinuityEmbedBuilder CreateBaseFailureEmbed(string? message, object errorCode) =>
            _embedBuilder.Create()
                .WithTitle(CommandResources.Title)
                .AddField(CommandResources.FieldTitleResult, CommandResources.FieldValueResultFailure, true)
                .AddField(CommandResources.FieldTitleErrorCode, errorCode, true)
                .AddField(CommandResources.FieldTitleReason, message ?? CommandResources.FieldValueReasonDefault)
                .WithThumbnailUrl(IconResources.Crossmark)
                .WithColor(Color.Red)
                .WithTimestamp();
    }
}
