using Discord;
using Injhinuity.Client.Core.Resources;
using Injhinuity.Client.Discord.Builders;

namespace Injhinuity.Client.Discord.Embeds.Factories
{
    public interface IPermissionEmbedBuilderFactory
    {
        EmbedBuilder CreateMissingPermissionFailure();
    }

    public class PermissionEmbedBuilderFactory : IPermissionEmbedBuilderFactory
    {
        private readonly IInjhinuityEmbedBuilder _embedBuilder;

        public PermissionEmbedBuilderFactory(IInjhinuityEmbedBuilder embedBuilder)
        {
            _embedBuilder = embedBuilder;
        }

        public EmbedBuilder CreateMissingPermissionFailure() =>
            _embedBuilder.Create()
                .WithTitle(PermissionResources.TitleMissingPermission)
                .WithDescription(PermissionResources.DescMissingPermission)
                .WithThumbnailUrl(IconResources.Crossmark)
                .WithColor(Color.Red)
                .WithTimestamp()
                .Build();
    }
}
