using AutoFixture;
using FluentAssertions;
using Injhinuity.Client.Services.Factories;
using Xunit;

namespace Injhinuity.Client.Tests.Services.Factories
{
    public class CommandBundleFactoryTests
    {
        private static readonly IFixture Fixture = new Fixture();
        private readonly ICommandBundleFactory _subject;

        private readonly string _guildId = Fixture.Create<string>();
        private readonly string _name = Fixture.Create<string>();
        private readonly string _body = Fixture.Create<string>();

        public CommandBundleFactoryTests()
        {
            _subject = new CommandBundleFactory();
        }

        [Fact]
        public void Create_WithGuildId_ThenCallsAppropriateFunction()
        {
            var result = _subject.Create(_guildId);

            result.GuildId.Should().Be(_guildId);
            result.Request.Should().BeNull();
        }

        [Fact]
        public void Create_WithGuildIdAndName_ThenCallsAppropriateFunction()
        {
            var result = _subject.Create(_guildId, _name);

            result.GuildId.Should().Be(_guildId);
            result.Request.Name.Should().Be(_name);
            result.Request.Body.Should().BeEmpty();
        }

        [Fact]
        public void Create_GuildIdAndNameAndBody_ThenCallsAppropriateFunction()
        {
            var result = _subject.Create(_guildId, _name, _body);

            result.GuildId.Should().Be(_guildId);
            result.Request.Name.Should().Be(_name);
            result.Request.Body.Should().Be(_body);
        }
    }
}
