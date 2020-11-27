using System.Collections.Generic;
using System.Linq;
using Discord;
using FluentAssertions;
using Injhinuity.Client.Core.Resources;
using Injhinuity.Client.Discord.Embeds.Content;
using Xunit;

namespace Injhinuity.Client.Tests.Discord.Embeds.Content
{
    public class DescriptionListEmbedContentTests
    {
        private DescriptionListEmbedContent _subject;

        private readonly EmbedBuilder _embedBuilder = new EmbedBuilder();

        private readonly IEnumerable<string> _emptyRows = Enumerable.Empty<string>();
        private readonly IEnumerable<string> _rows = new[] { "row1", "row2" };

        [Fact]
        public void DescriptionListEmbedContent_WithNoRows_ThenBuildsItsContent()
        {
            _subject = new DescriptionListEmbedContent(_embedBuilder, _emptyRows);

            var result = _subject.Get();

            result.Description.Should().Be(CommonResources.FieldValueEmpty);
        }

        [Fact]
        public void DescriptionListEmbedContent_WithRows_ThenBuildsItsContent()
        {
            _subject = new DescriptionListEmbedContent(_embedBuilder, _rows);

            var result = _subject.Get();

            result.Description.Should().Be("row1\nrow2\n");
        }
    }
}
