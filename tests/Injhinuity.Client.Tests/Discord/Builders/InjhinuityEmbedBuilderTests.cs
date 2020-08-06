using System.Collections;
using System.Collections.Generic;
using AutoFixture;
using Discord;
using FluentAssertions;
using FluentAssertions.Execution;
using Injhinuity.Client.Discord.Builders;
using Xunit;

namespace Injhinuity.Client.Tests.Discord.builders
{
    public class InjhinuityEmbedBuilderTests
    {
        private static readonly IFixture Fixture = new Fixture();
        private readonly IInjhinuityEmbedBuilder _subject;

        private static readonly string _title = Fixture.Create<string>();
        private static readonly string _desc = Fixture.Create<string>();
        private static readonly string _name = Fixture.Create<string>();
        private static readonly string _value = Fixture.Create<string>();
        private static readonly string _url = "https://i.imgur.com/wSTFkRM.png";
        private static readonly Color _color = Fixture.Create<Color>();

        public InjhinuityEmbedBuilderTests()
        {
            _subject = new InjhinuityEmbedBuilder();
        }

        [Theory]
        [ClassData(typeof(TestData))]
        public void Build_WhenCalledWithValues_ThenBuildsItsEmbedProperly(string title, string desc, string name, string value, string url, Color color)
        {
            var result = _subject.Create()
                .WithTitle(title)
                .WithDescription(desc)
                .WithThumbnailUrl(url)
                .WithColor(color)
                .AddField(name, value, true)
                .WithTimestamp()
                .Build();

            using var scope = new AssertionScope();

            result.Title.Should().Be(title);
            result.Description.Should().Be(desc);
            result.ThumbnailUrl.Should().Be(url);
            result.Timestamp.Should().NotBeNull();
            result.Color.Should().Be(color);

            result.Fields.Should().NotBeEmpty();
            result.Fields[0].Name.Should().Be(name);
            result.Fields[0].Value.Should().Be(value);
            result.Fields[0].IsInline.Should().Be(true);
        }

        [Theory]
        [ClassData(typeof(TestData))]
        public void Build_WhenCalledWithoutTimestamp_ThenBuildsItsEmbedProperly(string title, string desc, string name, string value, string url, Color color)
        {
            var result = _subject.Create()
                .WithTitle(title)
                .WithDescription(desc)
                .WithThumbnailUrl(url)
                .WithColor(color)
                .AddField(name, value, true)
                .Build();

            using var scope = new AssertionScope();

            result.Title.Should().Be(title);
            result.Description.Should().Be(desc);
            result.ThumbnailUrl.Should().Be(url);
            result.Timestamp.Should().BeNull();
            result.Color.Should().Be(color);

            result.Fields.Should().NotBeEmpty();
            result.Fields[0].Name.Should().Be(name);
            result.Fields[0].Value.Should().Be(value);
            result.Fields[0].IsInline.Should().Be(true);
        }

        private class TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { _title, _desc, _name, _value, _url, _color };
                yield return new object[] { null, _desc, _name, _value, _url, _color };
                yield return new object[] { _title, null, _name, _value, _url, _color };
                yield return new object[] { _title, _desc, _name, _value, _url, null };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
