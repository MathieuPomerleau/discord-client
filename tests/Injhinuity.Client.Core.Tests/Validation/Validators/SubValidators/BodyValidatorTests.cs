﻿using FluentAssertions;
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
    public class BodyValidatorTests
    {
        private readonly BodyValidator _subject;

        private readonly long _maxLength = 10;

        private readonly IValidationResultBuilder _validationResultBuilder;
        private readonly ILinkedValidator _linkedValidator;

        public BodyValidatorTests()
        {
            _linkedValidator = Substitute.For<ILinkedValidator>();

            _validationResultBuilder = new ValidationResultBuilder();
            _subject = new BodyValidator(_validationResultBuilder, _maxLength);
        }

        [Fact]
        public void Validate_WithAValidResource_ThenReturnsOkResult()
        {
            var resource = new CommandResource("name", "body");

            var result = _subject.Validate(resource);

            using var scope = new AssertionScope();
            result.Message.Should().BeNull();
            result.ValidationCode.Should().Be(ValidationCode.Ok);
        }

        [Fact]
        public void Validate_WithAnInvalidResourceType_ThenReturnsParseErrorResult()
        {
            var resource = new DummyResource();

            var result = _subject.Validate(resource);

            using var scope = new AssertionScope();
            result.Message.Should().Be(ValidationResources.ParseError);
            result.ValidationCode.Should().Be(ValidationCode.ParseError);
        }

        [Fact]
        public void Validate_WithAnEmptyContent_ThenReturnsValidationErrorResult()
        {
            var resource = new CommandResource("name", "");

            var result = _subject.Validate(resource);

            using var scope = new AssertionScope();
            result.Message.Should().Be(ValidationResources.BodyEmpty);
            result.ValidationCode.Should().Be(ValidationCode.ValidationError);
        }

        [Fact]
        public void Validate_WithAContentOverMaximumLength_ThenReturnsValidationErrorResult()
        {
            var resource = new CommandResource("name", "verylongbody");

            var result = _subject.Validate(resource);

            using var scope = new AssertionScope();
            result.Message.Should().Contain(ValidationResources.BodyTooLong[0..15]);
            result.ValidationCode.Should().Be(ValidationCode.ValidationError);
        }

        [Fact]
        public void Validate_WithSuccessAndHasALinkedValidator_ThenReturnsTheLinkedValidatorResult()
        {
            var resource = new CommandResource("name", "body");
            _subject.Next = _linkedValidator;
            _linkedValidator.Validate(Arg.Any<IValidationResource>()).Returns(new ValidationResult(ValidationCode.Ok));

            var result = _subject.Validate(resource);

            using var scope = new AssertionScope();
            result.Message.Should().BeNull();
            result.ValidationCode.Should().Be(ValidationCode.Ok);
        }
    }
}
