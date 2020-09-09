using Discord;
using Injhinuity.Client.Core.Resources;
using Injhinuity.Client.Discord.Builders;

namespace Injhinuity.Client.Discord.Embeds.Factories
{
    public interface IInformationEmbedBuilderFactory
    {
        EmbedBuilder CreateInfo(string versionNo);
    }

    public class InformationEmbedBuilderFactory : IInformationEmbedBuilderFactory
    {
        private readonly IInjhinuityEmbedBuilder _embedBuilder;

        public InformationEmbedBuilderFactory(IInjhinuityEmbedBuilder embedBuilder)
        {
            _embedBuilder = embedBuilder;
        }

        public EmbedBuilder CreateInfo(string versionNo) =>
            _embedBuilder
                .WithTitle(InformationResources.Title)
                .AddField(InformationResources.FieldTitleVersion, versionNo, true)
                .WithThumbnailUrl(IconResources.Information)
                .WithColor(Color.Blue)
                .WithTimestamp()
                .Build();
    }
}
