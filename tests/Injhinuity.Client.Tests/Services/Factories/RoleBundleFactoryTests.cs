using AutoFixture;
using Discord;
using FluentAssertions;
using FluentAssertions.Execution;
using Injhinuity.Client.Services.Factories;
using NSubstitute;
using Xunit;

namespace Injhinuity.Client.Tests.Services.Factories
{
    public class RoleBundleFactoryTests
    {
        private static readonly IFixture Fixture = new Fixture();
        private readonly IRoleBundleFactory _subject;

        private readonly string _guildId = Fixture.Create<string>();
        private readonly string _name = Fixture.Create<string>();
        private readonly string _emote = Fixture.Create<string>();
        private readonly ulong _id = Fixture.Create<ulong>();

        private readonly IRole _role;

        public RoleBundleFactoryTests()
        {
            _role = Substitute.For<IRole>();
            _role.Name.Returns(_name);
            _role.Id.Returns(_id);

            _subject = new RoleBundleFactory();
        }

        [Fact]
        public void Create_WithGuildId_ThenCallsAppropriateFunction()
        {
            var result = _subject.Create(_guildId);

            using var scope = new AssertionScope();
            result.GuildId.Should().Be(_guildId);
            result.Request.Should().BeNull();
        }

        [Fact]
        public void Create_ARole_ThenCallsAppropriateFunction()
        {
            var result = _subject.Create(_guildId, _role.Id.ToString(), _role.Name, _emote);

            using var scope = new AssertionScope();
            result.GuildId.Should().Be(_guildId);
            result.Request.Id.Should().Be(_id.ToString());
            result.Request.Name.Should().Be(_name);
            result.Request.EmoteString.Should().Be(_emote);
        }
    }
}
