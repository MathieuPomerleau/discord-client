using System.Net.Http;
using System.Threading.Tasks;
using Injhinuity.Client.Entities.Payloads;
using Injhinuity.Client.Enums;
using Injhinuity.Client.Model.Domain.Requests.Bundles;
using Injhinuity.Client.Services.Api;

namespace Injhinuity.Client.Services.Requesters
{
    public interface IGuildRequester
    {
        Task<HttpResponseMessage> ExecuteAsync(ApiAction action, GuildRequestBundle bundle);
    }

    public class GuildRequester : IGuildRequester
    {
        private readonly IApiGateway _apiGateway;
        private readonly IApiUrlProvider _apiUrlProvider;

        public GuildRequester(IApiGateway apiGateway, IApiUrlProvider apiUrlProvider)
        {
            _apiGateway = apiGateway;
            _apiUrlProvider = apiUrlProvider;
        }

        public Task<HttpResponseMessage> ExecuteAsync(ApiAction action, GuildRequestBundle bundle)
        {
            var url = _apiUrlProvider.GetFormattedUrl(action, bundle);
            var payload = new ApiPayload(url, bundle.Request);
            return _apiGateway.CallAsync(action, payload);
        }
    }
}
