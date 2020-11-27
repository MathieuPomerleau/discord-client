using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using Injhinuity.Client.Core.Resources;

namespace Injhinuity.Client.Discord.Embeds.Content
{
    public class DescriptionListEmbedContent : IEmbedContent
    {
        private readonly EmbedBuilder _embedBuilder;

        public DescriptionListEmbedContent(EmbedBuilder embedBuilder, IEnumerable<string> rows)
        {
            _embedBuilder = embedBuilder;
            BuildDescription(rows);
        }

        public EmbedBuilder Get() => _embedBuilder;

        private void BuildDescription(IEnumerable<string> rows)
        {
            if (!rows.Any())
            {
                _embedBuilder.Description = CommonResources.FieldValueEmpty;
            }
            else
            {
                var stringBuilder = new StringBuilder();
                foreach (var row in rows)
                    stringBuilder.Append($"{row}\n");

                _embedBuilder.Description = stringBuilder.ToString();
            }
        }
    }
}
