using AutoFixture;
using Discord;
using FluentAssertions;
using FluentAssertions.Execution;
using Injhinuity.Client.Discord.Factories;
using Xunit;

namespace Injhinuity.Client.Tests.Discord.Factories
{
    public class ActivityFactoryTests
    {
        private static readonly IFixture Fixture = new Fixture();
        private readonly IActivityFactory _subject;

        public ActivityFactoryTests()
        {
            _subject = new ActivityFactory();
        }

        [Fact]
        public void CreatePlayingStatus_ThenReturnsTheRightTypeOfIActivity()
        {
            var name = Fixture.Create<string>();

            var result = _subject.CreatePlayingStatus(name);

            using var scope = new AssertionScope();
            result.Should().BeOfType<Game>();
            result.Name.Should().Be(name);
        }
    }
}
