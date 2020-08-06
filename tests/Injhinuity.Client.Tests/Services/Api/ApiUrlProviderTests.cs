using System;
using System.Collections;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Injhinuity.Client.Enums;
using Injhinuity.Client.Model.Domain.Requests.Bundles;
using Injhinuity.Client.Services.Api;
using Xunit;

namespace Injhinuity.Client.Tests.Services.Api
{
    public class ApiUrlProviderTests
    {
        private static readonly IFixture Fixture = new Fixture();
        private readonly IApiUrlProvider _subject;

        private readonly CommandRequestBundle _commandBundle = Fixture.Create<CommandRequestBundle>();

        public ApiUrlProviderTests()
        {
            _subject = new ApiUrlProvider();
        }

        [Theory]
        [ClassData(typeof(TestData))]
        public void GetFormattedUrl_WhenCalledWithAnActionAndCommandBundle_ThenReturnsACommandUrl(ApiAction apiAction, string pathPart)
        {
            var result = _subject.GetFormattedUrl(apiAction, _commandBundle);

            result.Contains(pathPart).Should().BeTrue();
        }

        [Fact]
        public void GetFormattedUrl_WhenCalledWithAnInvalidActionAndCommandBundle_ThenThrowsAnException()
        {
            Func<string> act = () => _subject.GetFormattedUrl((ApiAction) 999, _commandBundle);

            act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("Specified argument was out of the range of valid values. (Parameter '999')");
        }

        private class TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { ApiAction.Delete, "command/" };
                yield return new object[] { ApiAction.Get, "command/" };
                yield return new object[] { ApiAction.GetAll, "commands" };
                yield return new object[] { ApiAction.Post, "commands" };
                yield return new object[] { ApiAction.Put, "commands" };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
