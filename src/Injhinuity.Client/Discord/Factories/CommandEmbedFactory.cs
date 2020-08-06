using Discord;
using Injhinuity.Client.Core.Exceptions;
using Injhinuity.Client.Core.Resources;
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
            CreateBaseFailureEmbed()
                .WithTitle(CommandResources.TitleCustom)
                .AddField(CommandResources.FieldTitleApiErrorCode, wrapper.StatusCode, true)
                .AddField(CommandResources.FieldTitleReason, wrapper.Reason ?? CommandResources.FieldValueReasonDefault)
                .Build();

        public EmbedBuilder CreateFailureEmbedBuilder(ExceptionWrapper wrapper) =>
            CreateBaseFailureEmbed()
                .AddField(CommandResources.FieldTitleApiErrorCode, wrapper.StatusCode, true)
                .AddField(CommandResources.FieldTitleReason, wrapper.Reason ?? CommandResources.FieldValueReasonDefault)
                .Build();

        private IInjhinuityEmbedBuilder CreateBaseSuccessEmbed() =>
            _embedBuilder.Create()
                .WithTitle(CommandResources.Title)
                .AddField(CommandResources.FieldTitleResult, CommandResources.FieldValueResultSuccess, true)
                .WithThumbnailUrl(IconResources.Checkmark)
                .WithColor(Color.Green)
                .WithTimestamp();

        private IInjhinuityEmbedBuilder CreateBaseFailureEmbed() =>
            _embedBuilder.Create()
                .WithTitle(CommandResources.Title)
                .AddField(CommandResources.FieldTitleResult, CommandResources.FieldValueResultFailure, true)
                .WithThumbnailUrl(IconResources.Crossmark)
                .WithColor(Color.Red)
                .WithTimestamp();
    }
}
