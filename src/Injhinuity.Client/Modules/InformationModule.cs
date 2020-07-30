using System.Threading.Tasks;
using Discord.Commands;
using Injhinuity.Client.Core.Configuration;
using Injhinuity.Client.Discord.Builders;
using Injhinuity.Client.Services.EmbedFactories;

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
            var embed = _embedFactory.CreateInfoEmbed(_clientConfig.Version.VersionNo);
            var result = _resultBuilder.WithEmbed(embed).Build();

            return Task.FromResult((RuntimeResult)result);
        }
    }
}
