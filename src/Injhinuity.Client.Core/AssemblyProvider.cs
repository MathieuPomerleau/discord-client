using System.Reflection;

namespace Injhinuity.Client.Core
{
    public interface IAssemblyProvider
    {
        Assembly GetCallingAssembly();
    }

    public class AssemblyProvider : IAssemblyProvider
    {
        public Assembly GetCallingAssembly() =>
            Assembly.GetCallingAssembly();
    }
}
