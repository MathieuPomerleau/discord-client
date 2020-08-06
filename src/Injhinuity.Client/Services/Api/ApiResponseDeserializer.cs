using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Injhinuity.Client.Core.Exceptions;
using Injhinuity.Client.Services.Mappers;
using Newtonsoft.Json;

namespace Injhinuity.Client.Services.Api
{
    public interface IApiReponseDeserializer
    {
        Task<T> DeserializeAsync<T>(HttpResponseMessage message);
        Task<K?> DeserializeAndAdaptAsync<T, K>(HttpResponseMessage message) where T : class where K : class;
        Task<IEnumerable<K>?> DeserializeAndAdaptEnumerableAsync<T, K>(HttpResponseMessage message);
    }

    public class ApiResponseDeserializer : IApiReponseDeserializer
    {
        private readonly IInjhinuityMapper _mapper;

        public ApiResponseDeserializer(IInjhinuityMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<T> DeserializeAsync<T>(HttpResponseMessage message) =>
            JsonConvert.DeserializeObject<T>(await message.Content.ReadAsStringAsync())
                ?? throw new InjhinuityException("Deserialized result is null.");

        public async Task<K?> DeserializeAndAdaptAsync<T, K>(HttpResponseMessage message) where T : class where K : class =>
            _mapper.Map<T, K>(await DeserializeAsync<T>(message));

        public Task<IEnumerable<K>?> DeserializeAndAdaptEnumerableAsync<T, K>(HttpResponseMessage message) =>
            DeserializeAndAdaptAsync<IEnumerable<T>, IEnumerable<K>>(message);
    }
}
