using System.Threading.Tasks;
using Discord.Commands;
using Injhinuity.Client.Core.Configuration;
using Injhinuity.Client.Discord.Builders;
using Injhinuity.Client.Discord.Factories;

namespace Injhinuity.Client.Modules
{
    public class InformationModule : ModuleBase<SocketCommandContext>
    {
        private readonly ICommandResultBuilder _resultBuilder;
        private readonly IInformationEmbedFactory _embedFactory;
        private readonly IClientConfig _clientConfig;

        public InformationModule(ICommandResultBuilder resultBuilder, IInformationEmbedFactory embedFactory, IClientConfig clientConfig)
        {
            _resultBuilder = resultBuilder;
            _embedFactory = embedFactory;
            _clientConfig = clientConfig;
        }

        [Command("info")]
        public Task<RuntimeResult> InfoAsync()
        {
            var embedBuilder = _embedFactory.CreateInfoEmbedBuilder(_clientConfig.Version.VersionNo);
            var result = _resultBuilder.Create()
                .WithEmbedBuilder(embedBuilder)
                .Build();

            return Task.FromResult((RuntimeResult)result);
        }
    }
}
