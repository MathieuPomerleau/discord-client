using Discord.Commands;
using FluentAssertions;
using Injhinuity.Client.Discord.Entities;
using Injhinuity.Client.Discord.Factories;
using NSubstitute;
using Xunit;

namespace Injhinuity.Client.Tests.Discord.Factories
{
    public class InjhinuityCommandContextFactoryTests
    {
        private readonly IInjhinuityCommandContextFactory _subject;

        private readonly ICommandContext _context;

        public InjhinuityCommandContextFactoryTests()
        {
            _context = Substitute.For<ICommandContext>();

            _subject = new InjhinuityCommandContextFactory();
        }

        [Fact]
        public void Create_WhenCalledWithContext_ThenCreatesAValidContext()
        {
            var result = _subject.Create(_context);

            result.Should().BeOfType<InjhinuityCommandContext>();
        }
    }
}
