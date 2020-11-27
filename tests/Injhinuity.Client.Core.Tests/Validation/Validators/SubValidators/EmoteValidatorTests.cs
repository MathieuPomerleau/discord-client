using FluentAssertions;
using FluentAssertions.Execution;
using Injhinuity.Client.Core.Resources;
using Injhinuity.Client.Core.Tests.Utils;
using Injhinuity.Client.Core.Validation.Builders;
using Injhinuity.Client.Core.Validation.Entities;
using Injhinuity.Client.Core.Validation.Entities.Resources;
using Injhinuity.Client.Core.Validation.Entities.Resources.Interfaces;
using Injhinuity.Client.Core.Validation.Enums;
using Injhinuity.Client.Core.Validation.Validators.SubValidators;
using NSubstitute;
using Xunit;

namespace Injhinuity.Client.Core.Tests.Validation.Validators.SubValidators
{
    public class EmoteValidatorTests
    {
        private EmoteValidator _subject;

        private readonly ILinkedValidator _linkedValidator;
        private readonly IValidationResultBuilder _validationResultBuilder;

        public EmoteValidatorTests()
        {
            _linkedValidator = Substitute.For<ILinkedValidator>();
            _validationResultBuilder = new ValidationResultBuilder();
        }

        [Fact]
        public void Validate_WithAValidResource_ThenReturnsOkResult()
        {
            var resource = new RoleResource("name", "emote");
            _subject = new EmoteValidator(_validationResultBuilder, (resource) => true);

            var result = _subject.Validate(resource);

            using var scope = new AssertionScope();
            result.Message.Should().BeNull();
            result.ValidationCode.Should().Be(ValidationCode.Ok);
        }

        [Fact]
        public void Validate_WithAnInvalidResourceType_ThenReturnsParseErrorResult()
        {
            var resource = new DummyResource();
            _subject = new EmoteValidator(_validationResultBuilder, (resource) => true);

            var result = _subject.Validate(resource);

            using var scope = new AssertionScope();
            result.Message.Should().Be(ValidationResources.ParseError);
            result.ValidationCode.Should().Be(ValidationCode.ParseError);
        }

        [Fact]
        public void Validate_WithAnEmptyEmote_ThenReturnsValidationErrorResult()
        {
            var resource = new RoleResource("name", "");
            _subject = new EmoteValidator(_validationResultBuilder, (resource) => true);

            var result = _subject.Validate(resource);

            using var scope = new AssertionScope();
            result.Message.Should().Be(ValidationResources.EmoteEmpty);
            result.ValidationCode.Should().Be(ValidationCode.ValidationError);
        }

        [Fact]
        public void Validate_WithAnInvalidEmote_ThenReturnsValidationErrorResult()
        {
            var resource = new RoleResource("name", "invalid emote");
            _subject = new EmoteValidator(_validationResultBuilder, (resource) => false);

            var result = _subject.Validate(resource);

            using var scope = new AssertionScope();
            result.Message.Should().Be(ValidationResources.EmoteInvalidFormat);
            result.ValidationCode.Should().Be(ValidationCode.ValidationError);
        }

        [Fact]
        public void Validate_WithSuccessAndHasALinkedValidator_ThenReturnsTheLinkedValidatorResult()
        {
            var resource = new RoleResource("name", "emote");
            _subject = new EmoteValidator(_validationResultBuilder, (resource) => true);
            _subject.Next = _linkedValidator;
            _linkedValidator.Validate(Arg.Any<IValidationResource>()).Returns(new ValidationResult(ValidationCode.Ok));

            var result = _subject.Validate(resource);

            using var scope = new AssertionScope();
            result.Message.Should().BeNull();
            result.ValidationCode.Should().Be(ValidationCode.Ok);
        }
    }
}
