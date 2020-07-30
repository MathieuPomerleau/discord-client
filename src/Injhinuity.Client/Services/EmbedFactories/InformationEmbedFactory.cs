using Discord;
using Injhinuity.Client.Core.Resources;
using Injhinuity.Client.Discord.Builders;

namespace Injhinuity.Client.Services.EmbedFactories
{
    public interface IInformationEmbedFactory
    {
        Embed CreateInfoEmbed(string versionNo);
    }

    public class InformationEmbedFactory : IInformationEmbedFactory
    {
        private readonly IInjhinuityEmbedBuilder _embedBuilder;

        public InformationEmbedFactory(IInjhinuityEmbedBuilder embedBuilder)
        {
            _embedBuilder = embedBuilder;
        }

        public Embed CreateInfoEmbed(string versionNo) =>
            _embedBuilder
                .WithTitle(InformationResources.Title)
                .AddField(InformationResources.FieldTitleVersion, versionNo, true)
                .WithThumbnailUrl(IconResources.Information)
                .WithColor(Color.Blue)
                .WithTimestamp()
                .Build();
    }
}
