using Injhinuity.Client.Model.Domain.Requests;

namespace Injhinuity.Client.Entities.Payloads
{
    public interface IApiPayload
    {
        string Url { get; }
        IRequest? Data { get; }
    }

    public class ApiPayload : IApiPayload
    {
        public string Url { get; }
        public IRequest? Data { get; }

        public ApiPayload(string url, IRequest? data = null)
        {
            Url = url;
            Data = data;
        }
    }
}
