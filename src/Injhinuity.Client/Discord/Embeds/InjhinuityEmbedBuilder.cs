using System.Collections.Generic;
using Discord;

namespace Injhinuity.Client.Discord.Embeds
{
    public interface IEmbedBuilder
    {
        IEmbedBuilder WithTitle(string title);
        IEmbedBuilder WithDescription(string description);
        IEmbedBuilder WithThumbnailUrl(string url);
        IEmbedBuilder WithColor(Color color);
        IEmbedBuilder WithTimestamp();
        IEmbedBuilder AddField(string name, object value, bool inline = false);
        Embed Build();
    }

    public class InjhinuityEmbedBuilder : IEmbedBuilder
    {
        private EmbedBuilder? _embedBuilder;

        private string? _title;
        private string? _description;
        private Color? _color;
        private string? _thumbnailUrl;
        private bool _hasTimestamp = false;
        private IList<FieldData> _fields = new List<FieldData>();

        public IEmbedBuilder WithTitle(string title)
        {
            _title = title;
            return this;
        }

        public IEmbedBuilder WithDescription(string description)
        {
            _description = description;
            return this;
        }

        public IEmbedBuilder WithColor(Color color)
        {
            _color = color;
            return this;
        }

        public IEmbedBuilder WithThumbnailUrl(string url)
        {
            _thumbnailUrl = url;
            return this;
        }

        public IEmbedBuilder WithTimestamp()
        {
            _hasTimestamp = true;
            return this;
        }

        public IEmbedBuilder AddField(string name, object value, bool inline = false)
        {
            _fields.Add(new FieldData { Name = name, Value = value, Inline = inline });
            return this;
        }

        public Embed Build()
        {
            _embedBuilder = new EmbedBuilder();

            if (_title is not null)
                _embedBuilder.WithTitle(_title);

            if (_description is not null)
                _embedBuilder.WithDescription(_description);

            if (_color is not null)
                _embedBuilder.WithColor(_color.Value);

            if (_thumbnailUrl is not null)
                _embedBuilder.WithThumbnailUrl(_thumbnailUrl);

            if (_hasTimestamp)
                _embedBuilder.WithCurrentTimestamp();

            foreach (var field in _fields)
                _embedBuilder.AddField(field.Name, field.Value, field.Inline);

            return _embedBuilder.Build();
        }

        private struct FieldData
        {
            public string Name { get; set; }
            public object Value { get; set; }
            public bool Inline { get; set; }
        }
    }
}
