using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Injhinuity.Client.Entities.Payloads;
using Injhinuity.Client.Enums;
using Newtonsoft.Json;

namespace Injhinuity.Client.Services.Api
{
    public interface IApiGateway
    {
        Task<HttpResponseMessage> CallAsync(ApiAction action, IApiPayload payload);
    }

    public class ApiGateway : IApiGateway
    {
        private const string HttpClientName = "injhinuity";
        private const string MediaType = "application/json";
        private static readonly Encoding Encoding = Encoding.UTF8;

        private readonly IHttpClientFactory _httpClientFactory;

        public ApiGateway(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public Task<HttpResponseMessage> CallAsync(ApiAction action, IApiPayload payload) =>
            action switch
            {
                ApiAction.Delete => DeleteAsync(payload),
                ApiAction.Get    => GetAsync(payload),
                ApiAction.GetAll => GetAsync(payload),
                ApiAction.Post   => PostAsync(payload),
                ApiAction.Put    => PutAsync(payload),
                _                => throw new NotImplementedException()
            };

        private Task<HttpResponseMessage> DeleteAsync(IApiPayload payload) => 
            GetClient().DeleteAsync(payload.Url);

        private Task<HttpResponseMessage> GetAsync(IApiPayload payload) => 
            GetClient().GetAsync(payload.Url);

        private Task<HttpResponseMessage> PostAsync(IApiPayload payload) => 
            GetClient().PostAsync(payload.Url, GetHttpContent(payload));

        private Task<HttpResponseMessage> PutAsync(IApiPayload payload) =>
            GetClient().PutAsync(payload.Url, GetHttpContent(payload));

        private HttpClient GetClient() =>
            _httpClientFactory.CreateClient(HttpClientName);

        private HttpContent GetHttpContent(IApiPayload payload) =>
            new StringContent(JsonConvert.SerializeObject(payload.Data), Encoding, MediaType);
    }
}
