using Discord;
using Injhinuity.Client.Core.Resources;
using Injhinuity.Client.Discord.Builders;

namespace Injhinuity.Client.Discord.Embeds.Factories
{
    public interface IPermissionEmbedBuilderFactory
    {
        EmbedBuilder CreateMissingUserPermissionFailure();
        EmbedBuilder CreateMissingBotPermissionFailure();
    }

    public class PermissionEmbedBuilderFactory : IPermissionEmbedBuilderFactory
    {
        private readonly IInjhinuityEmbedBuilder _embedBuilder;

        public PermissionEmbedBuilderFactory(IInjhinuityEmbedBuilder embedBuilder)
        {
            _embedBuilder = embedBuilder;
        }

        public EmbedBuilder CreateMissingUserPermissionFailure() =>
            _embedBuilder.Create()
                .WithTitle(PermissionResources.TitleMissingUserPermission)
                .WithDescription(PermissionResources.DescMissingUserPermission)
                .WithThumbnailUrl(IconResources.Crossmark)
                .WithColor(Color.Red)
                .Build();

        public EmbedBuilder CreateMissingBotPermissionFailure() =>
            _embedBuilder.Create()
                .WithTitle(PermissionResources.TitleMissingBotPermission)
                .WithDescription(PermissionResources.DescMissingBotPermission)
                .WithThumbnailUrl(IconResources.Crossmark)
                .WithColor(Color.Red)
                .Build();
    }
}
