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
    public class AdminEmbedBuilderFactoryTests
    {
        private static readonly IFixture Fixture = new Fixture();
        private readonly IAdminEmbedBuilderFactory _subject;

        private readonly string _username = Fixture.Create<string>();

        private readonly IInjhinuityEmbedBuilder _embedBuilder;

        public AdminEmbedBuilderFactoryTests()
        {
            _embedBuilder = Substitute.For<IInjhinuityEmbedBuilder>();
            _embedBuilder.ReturnsForAll(_embedBuilder);

            _subject = new AdminEmbedBuilderFactory(_embedBuilder);
        }

        [Fact]
        public void CreateBanSuccess_ThenReturnsABuiltEmbed()
        {
            _subject.CreateBanSuccess(_username);

            AssertSuccess(AdminResources.DescriptionUserBanned, _username);
        }

        [Fact]
        public void CreateUnbanSuccess_ThenReturnsABuiltEmbed()
        {
            _subject.CreateUnbanSuccess(_username);

            AssertSuccess(AdminResources.DescriptionUserUnbanned, _username);
        }

        [Fact]
        public void CreateKickSuccess_ThenReturnsABuiltEmbed()
        {
            _subject.CreateKickSuccess(_username);

            AssertSuccess(AdminResources.DescriptionUserKicked, _username);
        }

        [Fact]
        public void CreateKickFailure_ThenReturnsABuiltEmbed()
        {
            _subject.CreateKickFailure();

            AssertFailure(CommonResources.FieldValueUserNotInGuild);
        }

        [Fact]
        public void CreateMuteSuccess_ThenReturnsABuiltEmbed()
        {
            _subject.CreateMuteSuccess(_username);

            AssertSuccess(AdminResources.DescriptionUserMuted, _username);
        }

        [Fact]
        public void CreateMuteFailure_ThenReturnsABuiltEmbed()
        {
            _subject.CreateMuteFailure();

            AssertFailure(CommonResources.FieldValueUserNotInGuild);
        }

        [Fact]
        public void CreateUnmuteSuccess_ThenReturnsABuiltEmbed()
        {
            _subject.CreateUnmuteSuccess(_username);

            AssertSuccess(AdminResources.DescriptionUserUnmuted, _username);
        }

        [Fact]
        public void CreateUnmuteFailure_ThenReturnsABuiltEmbed()
        {
            _subject.CreateUnmuteFailure();

            AssertFailure(CommonResources.FieldValueUserNotInGuild);
        }

        [Fact]
        public void CreateUserNotValidFailure_ThenReturnsABuiltEmbed()
        {
            _subject.CreateUserNotValidFailure();

            AssertFailure(CommonResources.FieldValueUserNotValid);
        }

        private void AssertSuccess(string actionResource, string username)
        {
            _embedBuilder.Received().Create();
            _embedBuilder.Received().AddField(CommonResources.FieldValueResultSuccess, string.Format(actionResource, username));
            _embedBuilder.Received().WithThumbnailUrl(IconResources.Checkmark);
            _embedBuilder.Received().WithTitle(AdminResources.Title);
            _embedBuilder.Received().WithColor(Color.Green);
            _embedBuilder.Received().WithTimestamp();
            _embedBuilder.Received().Build();
        }

        private void AssertFailure(string failureValue)
        {
            _embedBuilder.Received().Create();
            _embedBuilder.Received().AddField(CommonResources.FieldTitleFailure, failureValue, true);
            _embedBuilder.Received().WithThumbnailUrl(IconResources.Crossmark);
            _embedBuilder.Received().WithTitle(AdminResources.Title);
            _embedBuilder.Received().WithColor(Color.Red);
            _embedBuilder.Received().WithTimestamp();
            _embedBuilder.Received().Build();
        }
    }
}
