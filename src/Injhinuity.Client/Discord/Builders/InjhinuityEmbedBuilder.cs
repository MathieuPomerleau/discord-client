using System.Collections.Generic;
using Discord;
using Injhinuity.Client.Discord.Embeds;

namespace Injhinuity.Client.Discord.Builders
{
    public interface IInjhinuityEmbedBuilder
    {
        IInjhinuityEmbedBuilder Create();
        IInjhinuityEmbedBuilder WithTitle(string title);
        IInjhinuityEmbedBuilder WithDescription(string description);
        IInjhinuityEmbedBuilder WithThumbnailUrl(string url);
        IInjhinuityEmbedBuilder WithColor(Color color);
        IInjhinuityEmbedBuilder AddField(string name, object value, bool inline = false);
        EmbedBuilder Build();
    }

    public class InjhinuityEmbedBuilder : IInjhinuityEmbedBuilder
    {
        private EmbedBuilder? _embedBuilder;

        private string? _title;
        private string? _description;
        private Color? _color;
        private string? _thumbnailUrl;
        private readonly IList<InjhinuityEmbedField> _fields = new List<InjhinuityEmbedField>();

        public IInjhinuityEmbedBuilder Create()
        {
            _title = null;
            _description = null;
            _color = null;
            _thumbnailUrl = null;
            _fields.Clear();
            return this;
        }

        public IInjhinuityEmbedBuilder WithTitle(string title)
        {
            _title = title;
            return this;
        }

        public IInjhinuityEmbedBuilder WithDescription(string description)
        {
            _description = description;
            return this;
        }

        public IInjhinuityEmbedBuilder WithColor(Color color)
        {
            _color = color;
            return this;
        }

        public IInjhinuityEmbedBuilder WithThumbnailUrl(string url)
        {
            _thumbnailUrl = url;
            return this;
        }

        public IInjhinuityEmbedBuilder AddField(string name, object value, bool inline = false)
        {
            _fields.Add(new InjhinuityEmbedField(name, value, inline));
            return this;
        }

        public EmbedBuilder Build()
        {
            _embedBuilder = new EmbedBuilder();
            _embedBuilder.WithCurrentTimestamp();

            if (!(_title is null))
                _embedBuilder.WithTitle(_title);

            if (!(_description is null))
                _embedBuilder.WithDescription(_description);

            if (!(_color is null))
                _embedBuilder.WithColor(_color.Value);

            if (!(_thumbnailUrl is null))
                _embedBuilder.WithThumbnailUrl(_thumbnailUrl);

            foreach (var field in _fields)
                _embedBuilder.AddField(field.Name, field.Value, field.Inline);

            return _embedBuilder;
        }
    }
}
