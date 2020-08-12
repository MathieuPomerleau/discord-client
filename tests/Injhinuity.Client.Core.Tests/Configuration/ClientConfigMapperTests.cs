using System;
using FluentAssertions;
using FluentAssertions.Execution;
using Injhinuity.Client.Core.Configuration;
using Injhinuity.Client.Core.Configuration.Options;
using Injhinuity.Client.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Injhinuity.Client.Core.Tests.Configuration
{
    public class ClientConfigMapperTests
    {
        private readonly IClientConfigMapper _subject;

        public ClientConfigMapperTests()
        {
            _subject = new ClientConfigMapper();
        }

        [Fact]
        public void MapFromNullableOptions_WhenCalledWithNullClientOptions_ThenThrowAnInjhinuityException()
        {
            Action action = () => _subject.MapFromNullableOptions(null);

            action.Should().Throw<InjhinuityException>().WithMessage("Configuration couldn't be built, options are null");
        }

        [Fact]
        public void MapFromNullableOptions_WhenCalledWithInvalidOptions_ThenThrowAnInjhinuityException()
        {
            var options = CreateValidOptions();
            options.Version = new VersionOptions();
            options.Discord = null;

            Action action = () => _subject.MapFromNullableOptions(options);

            action.Should().Throw<InjhinuityException>().WithMessage("The following values are missing from the configuration:\n Version - VersionNo\n Client - Discord");
        }

        [Fact]
        public void MapFromNullableOptions_WhenCalledWithValidOptions_ThenMapsToAConfigObject()
        {
            var options = CreateValidOptions();

            var result = _subject.MapFromNullableOptions(options);

            using var scope = new AssertionScope();

            result.Should().BeOfType<ClientConfig>();
            result.Discord.Should().NotBeNull();
            result.Discord.Token.Should().NotBeNull();
            result.Discord.Prefix.Should().NotBeNull();
            result.Version.Should().NotBeNull();
            result.Version.VersionNo.Should().NotBeNull();
            result.Logging.Should().NotBeNull();
            result.Logging.LogLevel.Should().NotBeNull();
            result.Api.Should().NotBeNull();
            result.Api.BaseUrl.Should().NotBeNull();
            result.Validation.Command.Should().NotBeNull();
        }

        private IClientOptions CreateValidOptions() =>
            new ClientOptions
            {
                Discord = new DiscordOptions { Token = "token", Prefix = '!' },
                Logging = new LoggingOptions { LogLevel = LogLevel.Information },
                Version = new VersionOptions { VersionNo = "0" },
                Api = new ApiOptions { BaseUrl = "url" },
                Validation = new ValidationOptions
                {
                    Command = new CommandValidationOptions
                    {
                        CommandNameMaxLength = 0,
                        CommandBodyMaxLength = 0
                    }
                }
            };
    }
}
