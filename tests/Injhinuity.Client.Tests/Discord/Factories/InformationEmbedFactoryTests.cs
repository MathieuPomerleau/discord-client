using AutoFixture;
using Discord;
using Injhinuity.Client.Core.Resources;
using Injhinuity.Client.Discord.Builders;
using Injhinuity.Client.Discord.Factories;
using NSubstitute;
using NSubstitute.Extensions;
using Xunit;

namespace Injhinuity.Client.Tests.Discord.Factories
{
    public class InformationEmbedFactoryTests
    {
        private static readonly IFixture Fixture = new Fixture();
        private readonly IInformationEmbedFactory _subject;

        private readonly string _versionNo = Fixture.Create<string>();

        private readonly IInjhinuityEmbedBuilder _embedBuilder;

        public InformationEmbedFactoryTests()
        {
            _embedBuilder = Substitute.For<IInjhinuityEmbedBuilder>();
            _embedBuilder.ReturnsForAll(_embedBuilder);

            _subject = new InformationEmbedFactory(_embedBuilder);
        }

        [Fact]
        public void CreateInfoEmbedBuilder_WhenCalled_ThenReturnsABuiltEmbed()
        {
            _subject.CreateInfoEmbedBuilder(_versionNo);

            Received.InOrder(() =>
            {
                _embedBuilder.WithTitle(InformationResources.Title);
                _embedBuilder.AddField(InformationResources.FieldTitleVersion, _versionNo, true);
                _embedBuilder.WithThumbnailUrl(IconResources.Information);
                _embedBuilder.WithColor(Color.Blue);
                _embedBuilder.WithTimestamp();
                _embedBuilder.Build();
            });
        }
    }
}
