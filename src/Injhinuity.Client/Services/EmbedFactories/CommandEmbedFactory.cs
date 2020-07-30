using System.Collections.Generic;
using Discord;
using Injhinuity.Client.Core.Exceptions;
using Injhinuity.Client.Core.Resources;
using Injhinuity.Client.Discord.Builders;
using Injhinuity.Client.Model.Domain;

namespace Injhinuity.Client.Services.EmbedFactories
{
    public interface ICommandEmbedFactory
    {
        Embed CreateCreateSuccessEmbed(string name, string body);
        Embed CreateDeleteSuccessEmbed(string name);
        Embed CreateUpdateSuccessEmbed(string name, string body);
        Embed CreateGetAllSuccessEmbed(IEnumerable<Command>? commands);
        Embed CreateFailureEmbed(ExceptionWrapper wrapper);
        Embed CreateCustomFailureEmbed(ExceptionWrapper wrapper);
    }

    public class CommandEmbedFactory : ICommandEmbedFactory
    {
        private readonly IInjhinuityEmbedBuilder _embedBuilder;

        public CommandEmbedFactory(IInjhinuityEmbedBuilder embedBuilder)
        {
            _embedBuilder = embedBuilder;
        }

        public Embed CreateCreateSuccessEmbed(string name, string body) =>
            CreateBaseSuccessEmbed()
                .AddField(CommandResources.FieldTitleType, CommandResources.FieldValueTypeCreate, true)
                .AddField(CommandResources.FieldTitleName, name)
                .AddField(CommandResources.FieldTitleContent, body, true)
                .Build();

        public Embed CreateDeleteSuccessEmbed(string name) =>
            CreateBaseSuccessEmbed()
                .AddField(CommandResources.FieldTitleType, CommandResources.FieldValueTypeDelete, true)
                .AddField(CommandResources.FieldTitleName, name)
                .Build();

        public Embed CreateUpdateSuccessEmbed(string name, string body) =>
            CreateBaseSuccessEmbed()
                .AddField(CommandResources.FieldTitleType, CommandResources.FieldValueTypeUpdate, true)
                .AddField(CommandResources.FieldTitleName, name)
                .AddField(CommandResources.FieldTitleContent, body, true)
                .Build();

        public Embed CreateGetAllSuccessEmbed(IEnumerable<Command>? commands)
        {
            var builder = _embedBuilder.Create()
                .WithTitle(CommandResources.TitlePlural)
                .WithThumbnailUrl(IconResources.List)
                .WithColor(Color.Orange)
                .WithTimestamp();

            if (commands is null)
            {
                builder.AddField(CommandResources.FieldTitleNoCommandsFound, CommandResources.FieldValueNoCommandsFound);
            }
            else
            {
                var counter = 0;
                foreach (var command in commands)
                    builder.AddField(command.Name, command.Body, counter++ % 3 != 0);
            }

            return builder.Build();
        }

        public Embed CreateCustomFailureEmbed(ExceptionWrapper wrapper) =>
            CreateBaseFailureEmbed()
                .WithTitle(CommandResources.TitleCustom)
                .AddField(CommandResources.FieldTitleApiErrorCode, wrapper.StatusCode, true)
                .AddField(CommandResources.FieldTitleReason, wrapper.Reason ?? CommandResources.FieldValueReasonDefault)
                .Build();

        public Embed CreateFailureEmbed(ExceptionWrapper wrapper) =>
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
