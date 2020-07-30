using AutoFixture;
using FluentAssertions;
using Injhinuity.Client.Services.Factories;
using Xunit;

namespace Injhinuity.Client.Tests.Services.Factories
{
    public class CommandPackageFactoryTests
    {
        private static readonly IFixture _fixture = new Fixture();
        private readonly ICommandPackageFactory _subject;

        private readonly string _guildId = _fixture.Create<string>();
        private readonly string _name = _fixture.Create<string>();
        private readonly string _body = _fixture.Create<string>();

        public CommandPackageFactoryTests()
        {
            _subject = new CommandPackageFactory();
        }

        [Fact]
        public void Create_WhenCalledWithGuildId_ThenCallsAppropriateFunction()
        {
            var result = _subject.Create(_guildId);

            result.GuildId.Should().Be(_guildId);
            result.Request.Should().BeNull();
        }

        [Fact]
        public void Create_WhenCalledWithGuildIdAndName_ThenCallsAppropriateFunction()
        {
            var result = _subject.Create(_guildId, _name);

            result.GuildId.Should().Be(_guildId);
            result.Request.Name.Should().Be(_name);
            result.Request.Body.Should().BeEmpty();
        }

        [Fact]
        public void Create_WhenCalledWithGuildIdAndNameAndBody_ThenCallsAppropriateFunction()
        {
            var result = _subject.Create(_guildId, _name, _body);

            result.GuildId.Should().Be(_guildId);
            result.Request.Name.Should().Be(_name);
            result.Request.Body.Should().Be(_body);
        }
    }
}
