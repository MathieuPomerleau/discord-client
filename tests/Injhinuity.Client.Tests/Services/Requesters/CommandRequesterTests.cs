using System.Threading.Tasks;
using AutoFixture;
using Injhinuity.Client.Entities.Payloads;
using Injhinuity.Client.Enums;
using Injhinuity.Client.Model.Domain.Requests.Bundles;
using Injhinuity.Client.Services.Api;
using Injhinuity.Client.Services.Requesters;
using NSubstitute;
using Xunit;

namespace Injhinuity.Client.Tests.Services.Requesters
{
    public class CommandRequesterTests
    {
        private static readonly IFixture _fixture = new Fixture();
        private readonly ICommandRequester _subject;

        private readonly CommandRequestBundle _requestPackage = _fixture.Create<CommandRequestBundle>();

        private readonly IApiGateway _apiGateway;
        private readonly IApiUrlProvider _apiUrlProvider;

        public CommandRequesterTests()
        {
            _apiGateway = Substitute.For<IApiGateway>();
            _apiUrlProvider = Substitute.For<IApiUrlProvider>();

            _subject = new CommandRequester(_apiGateway, _apiUrlProvider);
        }

        [Fact]
        public async Task ExecuteAsync_WhenCalled_ThenCallsApiGatewayWithPayload()
        {
            await _subject.ExecuteAsync(ApiAction.Get, _requestPackage);

            _apiUrlProvider.Received().GetFormattedUrl(ApiAction.Get, _requestPackage);
            await _apiGateway.Received().CallAsync(ApiAction.Get, Arg.Any<IApiPayload>());
        }
    }
}
