using Microsoft.Extensions.DependencyInjection;

namespace Injhinuity.Client.Core.Interfaces
{
    public interface IRegistry
    {
        void Register(IServiceCollection services);
    }
}
