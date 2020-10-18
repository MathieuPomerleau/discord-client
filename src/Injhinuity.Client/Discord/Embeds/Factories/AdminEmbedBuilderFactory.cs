using Discord;
using Injhinuity.Client.Core.Resources;
using Injhinuity.Client.Discord.Builders;

namespace Injhinuity.Client.Discord.Embeds.Factories
{
    public interface IAdminEmbedBuilderFactory 
    {
        EmbedBuilder CreateBanSuccess(string username);
        EmbedBuilder CreateUnbanSuccess(string username);
        EmbedBuilder CreateKickSuccess(string username);
        EmbedBuilder CreateKickFailure();
        EmbedBuilder CreateMuteSuccess(string username);
        EmbedBuilder CreateMuteFailure();
        EmbedBuilder CreateUnmuteSuccess(string username);
        EmbedBuilder CreateUnmuteFailure();
        EmbedBuilder CreateUserNotValidFailure();
    }

    public class AdminEmbedBuilderFactory : IAdminEmbedBuilderFactory
    {
        private readonly IInjhinuityEmbedBuilder _embedBuilder;

        public AdminEmbedBuilderFactory(IInjhinuityEmbedBuilder embedBuilder)
        {
            _embedBuilder = embedBuilder;
        }

        public EmbedBuilder CreateBanSuccess(string username) =>
            CreateBaseSuccessEmbed(AdminResources.DescriptionUserBanned, username)
                .Build();

        public EmbedBuilder CreateUnbanSuccess(string username) =>
            CreateBaseSuccessEmbed(AdminResources.DescriptionUserUnbanned, username)
                .Build();

        public EmbedBuilder CreateKickSuccess(string username) =>
            CreateBaseSuccessEmbed(AdminResources.DescriptionUserKicked, username)
                .Build();

        public EmbedBuilder CreateKickFailure() =>
            CreateBaseFailureEmbed(CommonResources.FieldValueUserNotInGuild)
                .Build();

        public EmbedBuilder CreateMuteSuccess(string username) =>
            CreateBaseSuccessEmbed(AdminResources.DescriptionUserMuted, username)
                .Build();

        public EmbedBuilder CreateMuteFailure() =>
            CreateBaseFailureEmbed(CommonResources.FieldValueUserNotInGuild)
                .Build();

        public EmbedBuilder CreateUnmuteSuccess(string username) =>
            CreateBaseSuccessEmbed(AdminResources.DescriptionUserUnmuted, username)
                .Build();

        public EmbedBuilder CreateUnmuteFailure() =>
            CreateBaseFailureEmbed(CommonResources.FieldValueUserNotInGuild)
                .Build();

        public EmbedBuilder CreateUserNotValidFailure() =>
            CreateBaseFailureEmbed(CommonResources.FieldValueUserNotValid)
                .WithTitle(AdminResources.Title)
                .Build();

        private IInjhinuityEmbedBuilder CreateBaseSuccessEmbed(string actionResource, string username) =>
            _embedBuilder.Create()
                .WithTitle(AdminResources.Title)
                .AddField(CommonResources.FieldValueResultSuccess, string.Format(actionResource, username))
                .WithThumbnailUrl(IconResources.Checkmark)
                .WithColor(Color.Green)
                .WithTimestamp();

        private IInjhinuityEmbedBuilder CreateBaseFailureEmbed(string failureValue) =>
            _embedBuilder.Create()
                .WithTitle(AdminResources.Title)
                .AddField(CommonResources.FieldTitleFailure, failureValue, true)
                .WithThumbnailUrl(IconResources.Crossmark)
                .WithColor(Color.Red)
                .WithTimestamp();
    }
}
