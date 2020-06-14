using AutoFixture;
using Discord;
using FluentAssertions;
using Injhinuity.Client.Discord.Activity;
using Xunit;

namespace Injhinuity.Client.Tests.Discord.Activity
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
            var str = _fixture.Create<string>();
            var result = _subject.CreatePlayingStatus(str);

            result.Should().BeOfType<Game>();
        }
    }
}
