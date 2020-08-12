using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using Injhinuity.Client.Core.Validation.Builders;
using Injhinuity.Client.Core.Validation.Enums;
using Xunit;

namespace Injhinuity.Client.Core.Tests.Validation.Builders
{
    public class ValidationResultBuilderTests
    {
        private readonly IValidationResultBuilder _subject;

        public ValidationResultBuilderTests()
        {
            _subject = new ValidationResultBuilder();
        }

        [Theory]
        [ClassData(typeof(TestData))]
        public void Build_WhenCalledWithValues_ThenProperlyBuildsResult(ValidationCode validationCode, string message, (string, string) replaceValue)
        {
            var result = _subject.Create()
                .WithCode(validationCode)
                .WithMessage(message)
                .WithReplacingValue(replaceValue.Item1, replaceValue.Item2)
                .Build();

            result.ValidationCode.Should().Be(validationCode);
            result.Message.Should().Contain(message[0..6]);
            result.Message.Should().Contain(replaceValue.Item2);
        }

        private class TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { ValidationCode.Ok, "message {tag}", ("{tag}", "value") };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
