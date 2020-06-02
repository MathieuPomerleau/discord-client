using System.Threading.Tasks;
using AutoFixture;
using Injhinuity.Client.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Injhinuity.Client.Tests
{
    public class InjhinuityClientTests
    {
        private static readonly IFixture _fixture = new Fixture();

        private readonly IInjhinuityClient _subject;

        private readonly IServiceCollection _services;
        private readonly ILogger<InjhinuityClient> _consoleLogger;
        private readonly IClientConfig _clientConfig;

        public InjhinuityClientTests()
        {
            _services = Substitute.For<IServiceCollection>();
            _consoleLogger = Substitute.For<ILogger<InjhinuityClient>>();
            _clientConfig = CreateClientConfig();

            _subject = new InjhinuityClient(_services, _consoleLogger, _clientConfig);
        }

        [Fact]
        public async Task RunAndBlockAsync_WhenCalled_ThenClientRegistersItself()
        {
            await _subject.RunAsync();

            _services.ReceivedWithAnyArgs().AddSingleton(_subject);
        }

        [Fact]
        public async Task RunAndBlockAsync_WhenCalled_ThenLogStartupMessageWithVersionNo()
        {
            await _subject.RunAsync();

            _consoleLogger.Received().LogInformation($"Launching Injhinuity version {_clientConfig.Version.VersionNo}");
        }

        private IClientConfig CreateClientConfig() =>
            new ClientConfig
            {
                Version = new VersionConfig { VersionNo = _fixture.Create<string>() },
                Logging = new LoggingConfig { }
            };
    }
}
