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
        public void CreateMissingUserPermissionFailure_ThenReturnsABuiltEmbed()
        {
            _subject.CreateMissingUserPermissionFailure();

            _embedBuilder.Received().Create();
            _embedBuilder.Received().WithThumbnailUrl(IconResources.Crossmark);
            _embedBuilder.Received().WithTitle(PermissionResources.TitleMissingUserPermission);
            _embedBuilder.Received().WithDescription(PermissionResources.DescMissingUserPermission);
            _embedBuilder.Received().WithColor(Color.Red);
            _embedBuilder.Received().Build();
        }

        [Fact]
        public void CreateMissingBotPermissionFailure_ThenReturnsABuiltEmbed()
        {
            _subject.CreateMissingBotPermissionFailure();

            _embedBuilder.Received().Create();
            _embedBuilder.Received().WithThumbnailUrl(IconResources.Crossmark);
            _embedBuilder.Received().WithTitle(PermissionResources.TitleMissingBotPermission);
            _embedBuilder.Received().WithDescription(PermissionResources.DescMissingBotPermission);
            _embedBuilder.Received().WithColor(Color.Red);
            _embedBuilder.Received().Build();
        }
    }
}
