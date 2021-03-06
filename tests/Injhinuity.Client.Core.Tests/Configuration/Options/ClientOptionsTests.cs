﻿using FluentAssertions;
using FluentAssertions.Execution;
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
                Logging = new LoggingOptions { AppLogLevel = LogLevel.Information, DiscordLogLevel = LogLevel.Information },
                Version = new VersionOptions { VersionNo = "0" },
                Api = new ApiOptions { BaseUrl = "url" },
                Validation = new ValidationOptions
                {
                    Command = new CommandValidationOptions
                    {
                        CommandBodyMaxLength = 0,
                        CommandNameMaxLength = 0
                    }
                }
            };
        }

        [Fact]
        public void ContainsNull_WithNonNullOptions_ThenResultIsValid()
        {
            var result = new NullableOptionsResult();

            _subject.ContainsNull(result);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ContainsNull_WithAtLeastOneNullOptions_ThenResultIsNotValid()
        {
            var result = new NullableOptionsResult();
            _subject.Discord = null;

            _subject.ContainsNull(result);

            using var scope = new AssertionScope();
            result.IsValid.Should().BeFalse();
            result.NullValues.Should().Contain(("Client", "Discord"));
        }
    }
}
