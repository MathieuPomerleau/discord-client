﻿using System;
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
        private readonly RoleRequestBundle _roleBundle = Fixture.Create<RoleRequestBundle>();
        private readonly GuildRequestBundle _guildBundle = Fixture.Create<GuildRequestBundle>();

        public ApiUrlProviderTests()
        {
            _subject = new ApiUrlProvider();
        }

        [Theory]
        [ClassData(typeof(CommandTestData))]
        public void GetFormattedUrl_WithAnActionAndCommandBundle_ThenReturnsACommandUrl(ApiAction apiAction, string pathPart)
        {
            var result = _subject.GetFormattedUrl(apiAction, _commandBundle);

            result.Should().Contain(pathPart);
        }

        [Fact]
        public void GetFormattedUrl_WithAnInvalidActionAndCommandBundle_ThenThrowsAnException()
        {
            Func<string> act = () => _subject.GetFormattedUrl((ApiAction) 999, _commandBundle);

            act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("Specified argument was out of the range of valid values. (Parameter '999')");
        }

        [Theory]
        [ClassData(typeof(RoleTestData))]
        public void GetFormattedUrl_WithAnActionAndRoleBundle_ThenReturnsARoleUrl(ApiAction apiAction, string pathPart)
        {
            var result = _subject.GetFormattedUrl(apiAction, _roleBundle);

            result.Should().Contain(pathPart);
        }

        [Fact]
        public void GetFormattedUrl_WithAnInvalidActionAndRoleBundle_ThenThrowsAnException()
        {
            Func<string> act = () => _subject.GetFormattedUrl((ApiAction)999, _roleBundle);

            act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("Specified argument was out of the range of valid values. (Parameter '999')");
        }

        [Theory]
        [ClassData(typeof(GuildTestData))]
        public void GetFormattedUrl_WithAnActionAndRoleBundle_ThenReturnsAGuildUrl(ApiAction apiAction, string pathPart)
        {
            var result = _subject.GetFormattedUrl(apiAction, _guildBundle);

            result.Should().Contain(pathPart);
        }

        [Fact]
        public void GetFormattedUrl_WithAnInvalidActionAndGuildBundle_ThenThrowsAnException()
        {
            Func<string> act = () => _subject.GetFormattedUrl((ApiAction)999, _guildBundle);

            act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("Specified argument was out of the range of valid values. (Parameter '999')");
        }

        private class CommandTestData : IEnumerable<object[]>
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

        private class RoleTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { ApiAction.Delete, "role/" };
                yield return new object[] { ApiAction.Get, "role/" };
                yield return new object[] { ApiAction.GetAll, "roles" };
                yield return new object[] { ApiAction.Post, "roles" };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class GuildTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { ApiAction.Get, "guild/" };
                yield return new object[] { ApiAction.GetAll, "guilds" };
                yield return new object[] { ApiAction.Post, "guilds" };
                yield return new object[] { ApiAction.Put, "guilds" };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
