using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Injhinuity.Client.Model.Domain.Requests;
using Injhinuity.Client.Services.Factories;
using Xunit;

namespace Injhinuity.Client.Tests.Services.Factories
{
    public class GuildBundleFactoryTests
    {
        private static readonly IFixture Fixture = new Fixture();
        private readonly IGuildBundleFactory _subject;

        private readonly string _guildId = Fixture.Create<string>();
        private readonly string _channelId = Fixture.Create<string>();
        private readonly string _messageId = Fixture.Create<string>();
        private readonly string _muteRoleId = Fixture.Create<string>();
        private readonly RoleGuildSettingsRequest _roleSettingsRequest;

        public GuildBundleFactoryTests()
        {
            _roleSettingsRequest = new RoleGuildSettingsRequest(_channelId, _messageId, _muteRoleId);
            _subject = new GuildBundleFactory();
        }

        [Fact]
        public void Create_WithGuildId_ThenCallsAppropriateFunction()
        {
            var result = _subject.Create(_guildId);

            using var scope = new AssertionScope();
            result.Id.Should().Be(_guildId);
            result.Request.Should().BeNull();
        }

        [Fact]
        public void Create_WithGuildIdAndRoleSettings_ThenCallsAppropriateFunction()
        {
            var result = _subject.Create(_guildId, _roleSettingsRequest);

            using var scope = new AssertionScope();
            result.Id.Should().Be(_guildId);
            result.Request.Should().NotBeNull();
            result.Request.Id.Should().Be(_guildId);
            result.Request.RoleSettings.ReactionRoleChannelId.Should().Be(_channelId);
            result.Request.RoleSettings.ReactionRoleMessageId.Should().Be(_messageId);
            result.Request.RoleSettings.MuteRoleId.Should().Be(_muteRoleId);
        }

        [Fact]
        public void CreateDefault_WithGuildId_ThenCallsAppropriateFunction()
        {
            var result = _subject.CreateDefault(_guildId);

            using var scope = new AssertionScope();
            result.Id.Should().Be(_guildId);
            result.Request.Should().NotBeNull();
            result.Request.Id.Should().Be(_guildId);
            result.Request.RoleSettings.ReactionRoleChannelId.Should().Be(string.Empty);
            result.Request.RoleSettings.ReactionRoleMessageId.Should().Be(string.Empty);
            result.Request.RoleSettings.MuteRoleId.Should().Be(string.Empty);
        }
    }
}
