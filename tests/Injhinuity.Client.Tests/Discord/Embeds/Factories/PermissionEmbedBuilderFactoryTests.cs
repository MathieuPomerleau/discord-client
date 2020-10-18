using Discord;
using Injhinuity.Client.Core.Resources;
using Injhinuity.Client.Discord.Builders;
using Injhinuity.Client.Discord.Embeds.Factories;
using NSubstitute;
using NSubstitute.Extensions;
using Xunit;

namespace Injhinuity.Client.Tests.Discord.Embeds.Factories
{
    public class PermissionEmbedBuilderFactoryTests
    {
        private readonly IPermissionEmbedBuilderFactory _subject;

        private readonly IInjhinuityEmbedBuilder _embedBuilder;

        public PermissionEmbedBuilderFactoryTests()
        {
            _embedBuilder = Substitute.For<IInjhinuityEmbedBuilder>();
            _embedBuilder.ReturnsForAll(_embedBuilder);

            _subject = new PermissionEmbedBuilderFactory(_embedBuilder);
        }

        [Fact]
        public void CreateMissingPermissionFailure_ThenReturnsABuiltEmbed()
        {
            _subject.CreateMissingPermissionFailure();

            _embedBuilder.Received().Create();
            _embedBuilder.Received().WithThumbnailUrl(IconResources.Crossmark);
            _embedBuilder.Received().WithTitle(PermissionResources.TitleMissingPermission);
            _embedBuilder.Received().WithDescription(PermissionResources.DescMissingPermission);
            _embedBuilder.Received().WithColor(Color.Red);
            _embedBuilder.Received().WithTimestamp();
            _embedBuilder.Received().Build();
        }
    }
}
