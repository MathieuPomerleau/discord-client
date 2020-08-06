using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Discord;
using FluentAssertions;
using Injhinuity.Client.Discord.Embeds;
using Injhinuity.Client.Discord.Embeds.Content;
using Xunit;

namespace Injhinuity.Client.Tests.Discord.Embeds.Content
{
    public class ListEmbedContentTests
    {
        private static readonly IFixture Fixture = new Fixture();
        private ListEmbedContent _subject;

        private readonly int _fieldsPerPage = 2;
        private readonly EmbedBuilder _embedBuilder = new EmbedBuilder();

        [Fact]
        public void Get_WhenCalledWithSamePages_ThenReturnsSameEmbedBuilder()
        {
            var fields = GetOnePageFields();
            _subject = new ListEmbedContent(_fieldsPerPage, fields, _embedBuilder);

            var result = _subject.Get();

            result.Fields[0].Name.Should().Be(fields[0].Name);
            result.Fields[1].Name.Should().Be(fields[1].Name);
        }

        [Fact]
        public void Get_WhenCalledWithDifferentPageAndNotMaxPage_ThenReturnsNewBuilderWithNewPageContent()
        {
            var fields = GetThreePageFields();
            _subject = new ListEmbedContent(_fieldsPerPage, fields, _embedBuilder);
            _subject.NextPage();

            var result = _subject.Get();

            result.Fields[0].Name.Should().Be(fields[2].Name);
            result.Fields[1].Name.Should().Be(fields[3].Name);
        }

        [Fact]
        public void Get_WhenCalledAfterNoOpPageChange_ThenReturnsSameBuilderAsBefore()
        {
            var fields = GetOnePageFields();
            _subject = new ListEmbedContent(_fieldsPerPage, fields, _embedBuilder);
            _subject.NextPage();

            var result = _subject.Get();

            result.Fields[0].Name.Should().Be(fields[0].Name);
            result.Fields[1].Name.Should().Be(fields[1].Name);
        }

        [Fact]
        public void NextPage_WhenCalledAndOnSinglepage_ThenDoesntChangePage()
        {
            var fields = GetOnePageFields();
            _subject = new ListEmbedContent(_fieldsPerPage, fields, _embedBuilder);
            var page = _subject.CurrentPage;

            _subject.NextPage();

            _subject.CurrentPage.Should().Be(page);
        }

        [Fact]
        public void NextPage_WhenCalledAndOnMaxPage_ThenDoesntChangePage()
        {
            var fields = GetThreePageFields();
            _subject = new ListEmbedContent(_fieldsPerPage, fields, _embedBuilder);
            _subject.NextPage();
            _subject.NextPage();
            var page = _subject.CurrentPage;

            _subject.NextPage();

            _subject.CurrentPage.Should().Be(page);
        }

        [Fact]
        public void NextPage_WhenCalledAndNotSinglePageNotMaxPage_ThenChangesPage()
        {
            var fields = GetThreePageFields();
            _subject = new ListEmbedContent(_fieldsPerPage, fields, _embedBuilder);
            _subject.NextPage();
            var page = _subject.CurrentPage;

            _subject.NextPage();

            _subject.CurrentPage.Should().Be(page + 1);
        }

        [Fact]
        public void PreviousPage_WhenCalledAndOnSinglepage_ThenDoesntChangePage()
        {
            var fields = GetOnePageFields();
            _subject = new ListEmbedContent(_fieldsPerPage, fields, _embedBuilder);
            var page = _subject.CurrentPage;

            _subject.PreviousPage();

            _subject.CurrentPage.Should().Be(page);
        }

        [Fact]
        public void PreviousPage_WhenCalledAndOnMinPage_ThenDoesntChangePage()
        {
            var fields = GetThreePageFields();
            _subject = new ListEmbedContent(_fieldsPerPage, fields, _embedBuilder);
            var page = _subject.CurrentPage;

            _subject.PreviousPage();

            _subject.CurrentPage.Should().Be(page);
        }

        [Fact]
        public void PreviousPage_WhenCalledAndNotSinglePageNotMaxPage_ThenChangesPage()
        {
            var fields = GetThreePageFields();
            _subject = new ListEmbedContent(_fieldsPerPage, fields, _embedBuilder);
            _subject.NextPage();
            var page = _subject.CurrentPage;

            _subject.PreviousPage();

            _subject.CurrentPage.Should().Be(page - 1);
        }

        private IList<InjhinuityEmbedField> GetOnePageFields() =>
            new[] { Fixture.Create<InjhinuityEmbedField>(), Fixture.Create<InjhinuityEmbedField>() };

        private IList<InjhinuityEmbedField> GetThreePageFields() =>
            Enumerable.Range(0, 6).Select(_ => Fixture.Create<InjhinuityEmbedField>()).ToList();
    }
}
