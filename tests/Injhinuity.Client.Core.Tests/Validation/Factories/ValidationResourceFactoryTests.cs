using FluentAssertions;
using Injhinuity.Client.Core.Validation.Entities.Resources;
using Injhinuity.Client.Core.Validation.Factories;
using Xunit;

namespace Injhinuity.Client.Core.Tests.Validation.Factories
{
    public class ValidationResourceFactoryTests
    {
        private readonly IValidationResourceFactory _subject;

        public ValidationResourceFactoryTests()
        {
            _subject = new ValidationResourceFactory();
        }

        [Fact]
        public void CreateCommand_WithValues_ThenBuildsItsResourceProperly()
        {
            var result = _subject.CreateCommand("name", "body");

            result.Should().BeOfType<CommandResource>();
        }

        [Fact]
        public void CreateRole_WithValues_ThenBuildsItsResourceProperly()
        {
            var result = _subject.CreateRole("name", "emote");

            result.Should().BeOfType<RoleResource>();
        }
    }
}
