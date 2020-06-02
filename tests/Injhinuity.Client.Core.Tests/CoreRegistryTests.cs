using Injhinuity.Client.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Injhinuity.Client.Core.Tests
{
    public class CoreRegistryTests
    {
        private readonly IRegistry _subject;

        private readonly IServiceCollection _services;

        public CoreRegistryTests()
        {
            _services = Substitute.For<IServiceCollection>();

            _subject = new CoreRegistry();
        }

        [Fact]
        public void Register_WhenCalled_ThenRegisterRightDependencies()
        {
        }
    }
}
