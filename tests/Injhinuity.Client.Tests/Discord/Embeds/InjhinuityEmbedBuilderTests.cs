using AutoFixture;
using Discord;
using FluentAssertions;
using FluentAssertions.Execution;
using Injhinuity.Client.Discord.Embeds;
using Xunit;

namespace Injhinuity.Client.Tests.Discord.Embeds
{
    public class InjhinuityEmbedBuilderTests
    {
        private static readonly IFixture _fixture = new Fixture();
        private readonly string _url = "https://i.imgur.com/wSTFkRM.png";
        private readonly Color _color = Color.Red;

        private readonly IEmbedBuilder _subject;

        public InjhinuityEmbedBuilderTests()
        {
            _subject = new InjhinuityEmbedBuilder();
        }

        [Fact]
        public void Build_WhenCalledWithValues_ThenBuildsItsEmbedProperly()
        {
            var (title, desc, name, value, url, color) = CreateEmbedValues();

            var result = _subject.WithTitle(title)
                .WithDescription(desc)
                .WithThumbnailUrl(url)
                .WithColor(color)
                .AddField(name, value, true)
                .WithTimestamp()
                .Build();

            using var scope = new AssertionScope();

            result.Title.Should().Be(title);
            result.Description.Should().Be(desc);
            result.Thumbnail.Value.Url.Should().Be(url);
            result.Timestamp.Should().NotBeNull();
            result.Color.Should().Be(color);

            result.Fields.Should().NotBeEmpty();
            result.Fields[0].Name.Should().Be(name);
            result.Fields[0].Value.Should().Be(value);
            result.Fields[0].Inline.Should().Be(true);
        }

        [Fact]
        public void Build_WhenCalledWithoutTimestamp_ThenBuildsItsEmbedProperly()
        {
            var (title, desc, name, value, url, color) = CreateEmbedValues();

            var result = _subject.WithTitle(title)
                .WithDescription(desc)
                .WithThumbnailUrl(url)
                .WithColor(color)
                .AddField(name, value, true)
                .Build();

            using var scope = new AssertionScope();

            result.Title.Should().Be(title);
            result.Description.Should().Be(desc);
            result.Thumbnail.Value.Url.Should().Be(url);
            result.Timestamp.Should().BeNull();
            result.Color.Should().Be(color);

            result.Fields.Should().NotBeEmpty();
            result.Fields[0].Name.Should().Be(name);
            result.Fields[0].Value.Should().Be(value);
            result.Fields[0].Inline.Should().Be(true);
        }

            private (string, string, string, string, string, Color) CreateEmbedValues() =>
            (_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(),
             _fixture.Create<string>(), _url, _color);
    }
}
