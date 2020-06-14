using Injhinuity.Client.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Injhinuity.Client.Core
{
    public class CoreRegistry : IRegistry
    {
        public void Register(IServiceCollection services)
        {
            services.AddSingleton<IAssemblyProvider, AssemblyProvider>();
        }
    }
}
