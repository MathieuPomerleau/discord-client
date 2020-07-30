using System.Collections.Generic;
using System.Linq;
using Mapster;

namespace Injhinuity.Client.Services.Mappers
{
    public interface IInjhinuityMapper
    {
        K? Map<T, K>(T? src) where T : class where K : class;
        IEnumerable<K>? MapEnumerable<T, K>(IEnumerable<T>? src) where T : class where K : class;
    }

    public class InjhinuityMapper : IInjhinuityMapper
    {
        public K? Map<T, K>(T? src) where T : class where K : class =>
            src?.Adapt<K>();

        public IEnumerable<K>? MapEnumerable<T, K>(IEnumerable<T>? src) where T : class where K : class =>
            src?.Select(x => x.Adapt<K>());
    }
}
