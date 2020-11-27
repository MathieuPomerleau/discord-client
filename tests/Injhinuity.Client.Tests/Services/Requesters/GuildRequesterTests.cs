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
    public class GuildRequesterTests
    {
        private static readonly IFixture Fixture = new Fixture();
        private readonly IGuildRequester _subject;
        private readonly GuildRequestBundle _requestBundle = Fixture.Create<GuildRequestBundle>();

        private readonly IApiGateway _apiGateway;
        private readonly IApiUrlProvider _apiUrlProvider;

        public GuildRequesterTests()
        {
            _apiGateway = Substitute.For<IApiGateway>();
            _apiUrlProvider = Substitute.For<IApiUrlProvider>();

            _subject = new GuildRequester(_apiGateway, _apiUrlProvider);
        }

        [Fact]
        public async Task ExecuteAsync_ThenCallsApiGatewayWithPayload()
        {
            await _subject.ExecuteAsync(ApiAction.Get, _requestBundle);

            _apiUrlProvider.Received().GetFormattedUrl(ApiAction.Get, _requestBundle);
            await _apiGateway.Received().CallAsync(ApiAction.Get, Arg.Any<IApiPayload>());
        }
    }
}
