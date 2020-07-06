using AutoFixture;
using Discord;
using FluentAssertions;
using Injhinuity.Client.Discord.Factory;
using Xunit;

namespace Injhinuity.Client.Tests.Discord.Factory
{
    public class ActivityFactoryTests
    {
        private static readonly IFixture _fixture = new Fixture();

        private readonly IActivityFactory _subject;

        public ActivityFactoryTests()
        {
            _subject = new ActivityFactory();
        }

        [Fact]
        public void CreatePlayingStatus_WhenCalled_ReturnsTheRightTypeOfIActivity()
        {
            var name = _fixture.Create<string>();

            var result = _subject.CreatePlayingStatus(name);

            result.Should().BeOfType<Game>();
            result.Name.Should().Be(name);
        }
    }
}
