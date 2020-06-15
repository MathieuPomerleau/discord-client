using System;
using FluentAssertions;
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
        public void MapFromNullableOptions_WhenCalledWithValidOptions_ThenMapsToAConfigObject()
        {
            var options = CreateValidOptions();

            var result = _subject.MapFromNullableOptions(options);

            result.Should().BeOfType<ClientConfig>();
            result.Discord.Should().NotBeNull();
            result.Discord.Token.Should().NotBeNull();
            result.Discord.Prefix.Should().NotBeNull();
            result.Version.Should().NotBeNull();
            result.Version.VersionNo.Should().NotBeNull();
            result.Logging.Should().NotBeNull();
            result.Logging.LogLevel.Should().NotBeNull();
        }

        [Fact]
        public void MapFromNullableOptions_WhenCalledWithInvalidOptions_ThenThrowAnInjhinuityException()
        {
            var options = CreateValidOptions();
            options.Discord = null;
            options.Version = null;
            options.Logging = null;

            Action action = () => _subject.MapFromNullableOptions(options);

            action.Should().Throw<InjhinuityException>().WithMessage("Config validation failed, missing value found");
        }

        private IClientOptions CreateValidOptions() =>
            new ClientOptions
            {
                Discord = new DiscordOptions { Token = "token", Prefix = '!' },
                Logging = new LoggingOptions { LogLevel = LogLevel.Information },
                Version = new VersionOptions { VersionNo = "0" }
            };
    }
}
