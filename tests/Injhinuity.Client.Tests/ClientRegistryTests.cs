using Injhinuity.Client.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Injhinuity.Client.Tests
{
    public class ClientRegistryTests
    {
        private readonly IRegistry _subject;

        private readonly IServiceCollection _services;

        public ClientRegistryTests()
        {
            _services = Substitute.For<IServiceCollection>();

            _subject = new ClientRegistry();
        }

        [Fact]
        public void Register_WhenCalled_ThenRegisterRightDependencies()
        {

        }
    }
}
