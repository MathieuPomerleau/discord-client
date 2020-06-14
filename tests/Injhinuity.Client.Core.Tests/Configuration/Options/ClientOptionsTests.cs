﻿using FluentAssertions;
using Injhinuity.Client.Core.Configuration.Options;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Injhinuity.Client.Core.Tests.Configuration.Options
{
    public class ClientOptionsTests
    {
        private readonly IClientOptions _subject;

        public ClientOptionsTests()
        {
            _subject = new ClientOptions
            {
                Discord = new DiscordOptions { Token = "token", Prefix = '!' },
                Logging = new LoggingOptions { LogLevel = LogLevel.Information },
                Version = new VersionOptions { VersionNo = "0" }
            };
        }

        [Fact]
        public void ContainsNull_WhenCalledWithNonNullOptions_ThenReturnsFalse()
        {
            var result = _subject.ContainsNull();

            result.Should().BeFalse();
        }

        [Fact]
        public void ContainsNull_WhenCalledWithAtLeastOneNullOptions_ThenReturnsTrue()
        {
            _subject.Discord = null;

            var result = _subject.ContainsNull();

            result.Should().BeTrue();
        }
    }
}