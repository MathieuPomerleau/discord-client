using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Injhinuity.Client.Core.Configuration;
using Injhinuity.Client.Discord.Embeds;
using Injhinuity.Client.Discord.Results;

namespace Injhinuity.Client.Modules
{
    public class InformationModule : ModuleBase<SocketCommandContext>
    {
        private readonly ICommandResultBuilder _resultBuilder;
        private readonly IEmbedBuilder _embedBuilder;
        private readonly IClientConfig _clientConfig;

        public InformationModule(ICommandResultBuilder resultBuilder, IEmbedBuilder embedBuilder, IClientConfig clientConfig)
        {
            _resultBuilder = resultBuilder;
            _embedBuilder = embedBuilder;
            _clientConfig = clientConfig;
        }

        [Command("info")]
        public Task<RuntimeResult> InfoAsync()
        {
            var embed = _embedBuilder.WithTitle("Information")
                .AddField("Current Version", _clientConfig.Version.VersionNo, true)
                .WithColor(Color.DarkRed)
                .WithTimestamp()
                .Build();

            var result = _resultBuilder.WithEmbed(embed).Build();

            return Task.FromResult((RuntimeResult)result);
        }
    }
}
