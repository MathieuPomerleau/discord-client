using AutoFixture;
using Discord;
using Injhinuity.Client.Core.Resources;
using Injhinuity.Client.Discord.Builders;
using Injhinuity.Client.Services.EmbedFactories;
using NSubstitute;
using NSubstitute.Extensions;
using Xunit;

namespace Injhinuity.Client.Tests.Services.EmbedFactories
{
    public class InformationEmbedFactoryTests
    {
        private static readonly IFixture _fixture = new Fixture();
        private readonly IInformationEmbedFactory _subject;

        private readonly string _versionNo = _fixture.Create<string>();

        private readonly IInjhinuityEmbedBuilder _embedBuilder;

        public InformationEmbedFactoryTests()
        {
            _embedBuilder = Substitute.For<IInjhinuityEmbedBuilder>();
            _embedBuilder.ReturnsForAll(_embedBuilder);

            _subject = new InformationEmbedFactory(_embedBuilder);
        }

        [Fact]
        public void CreateInfoEmbed_WhenCalled_ThenReturnsABuiltEmbed()
        {
            _subject.CreateInfoEmbed(_versionNo);

            _embedBuilder.Received().AddField(InformationResources.FieldTitleVersion, _versionNo, true);
            _embedBuilder.Received().WithThumbnailUrl(IconResources.Information);
            _embedBuilder.Received().WithTitle(InformationResources.Title);
            _embedBuilder.Received().WithColor(Color.Blue);
            _embedBuilder.Received().WithTimestamp();
            _embedBuilder.Received().Build();
        }
    }
}
