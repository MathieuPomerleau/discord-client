using AutoFixture;
using Discord;
using Injhinuity.Client.Core.Resources;
using Injhinuity.Client.Discord.Builders;
using Injhinuity.Client.Discord.Embeds.Factories;
using NSubstitute;
using NSubstitute.Extensions;
using Xunit;

namespace Injhinuity.Client.Tests.Discord.Embeds.Factories
{
    public class InformationEmbedBuilderFactoryTests
    {
        private static readonly IFixture Fixture = new Fixture();
        private readonly IInformationEmbedBuilderFactory _subject;

        private readonly string _versionNo = Fixture.Create<string>();

        private readonly IInjhinuityEmbedBuilder _embedBuilder;

        public InformationEmbedBuilderFactoryTests()
        {
            _embedBuilder = Substitute.For<IInjhinuityEmbedBuilder>();
            _embedBuilder.ReturnsForAll(_embedBuilder);

            _subject = new InformationEmbedBuilderFactory(_embedBuilder);
        }

        [Fact]
        public void CreateInfo_ThenReturnsABuiltEmbed()
        {
            _subject.CreateInfo(_versionNo);

            Received.InOrder(() =>
            {
                _embedBuilder.WithTitle(InformationResources.Title);
                _embedBuilder.AddField(InformationResources.FieldTitleVersion, _versionNo, true);
                _embedBuilder.WithThumbnailUrl(IconResources.Information);
                _embedBuilder.WithColor(Color.Blue);
                _embedBuilder.Build();
            });
        }
    }
}
