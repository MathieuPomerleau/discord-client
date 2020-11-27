using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Injhinuity.Client.Discord.Builders;
using Injhinuity.Client.Discord.Embeds.Factories;
using Injhinuity.Client.Discord.Factories;
using Injhinuity.Client.Services.Api;
using Injhinuity.Client.Services.Mappers;

namespace Injhinuity.Client.Modules
{
    public class AdminModule : BaseModule
    {
        public AdminModule(IInjhinuityCommandContextFactory commandContextFactory, IApiReponseDeserializer deserializer,
            ICommandResultBuilder resultBuilder, IEmbedBuilderFactoryProvider embedBuilderFactoryProvider,
            IInjhinuityMapper mapper)
            : base(commandContextFactory, deserializer, resultBuilder, mapper, embedBuilderFactoryProvider)
        {
        }

        [Command("ban")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task<RuntimeResult> BanAsync(IUser user)
        {
            await CustomContext.Guild.AddBanAsync(user);

            var embedBuilder = EmbedBuilderFactoryProvider.Admin.CreateBanSuccess(user.Username);
            return EmbedResult(embedBuilder);
        }

        [Command("unban")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task<RuntimeResult> UnbanAsync(IUser user)
        {
            await CustomContext.Guild.RemoveBanAsync(user);

            var embedBuilder = EmbedBuilderFactoryProvider.Admin.CreateUnbanSuccess(user.Username);
            return EmbedResult(embedBuilder);
        }

        [Command("kick")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task<RuntimeResult> KickAsync(IUser user)
        {
            var guildUser = await CustomContext.Guild.GetUserAsync(user.Id);
            await guildUser.KickAsync();

            var embedBuilder = EmbedBuilderFactoryProvider.Admin.CreateKickSuccess(user.Username);
            return EmbedResult(embedBuilder);
        }

        [Command("mute")]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        public async Task<RuntimeResult> MuteAsync(IUser user)
        {
            var guildUser = await CustomContext.Guild.GetUserAsync(user.Id);
            if (guildUser != null)
            {
                // await guildUser.KickAsync();
                // TODO: Grab mute role, check if exists, etc.
                var successEmbedBuilder = EmbedBuilderFactoryProvider.Admin.CreateMuteSuccess(user.Username);
                return EmbedResult(successEmbedBuilder);
            }

            var failureEmbedBuilder = EmbedBuilderFactoryProvider.Admin.CreateMuteFailure();
            return EmbedResult(failureEmbedBuilder);
        }

        [Command("unmute")]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        public async Task<RuntimeResult> UnmuteAsync(IUser user)
        {
            var guildUser = await CustomContext.Guild.GetUserAsync(user.Id);
            if (guildUser != null)
            {
                // await guildUser.KickAsync();
                // TODO: Grab mute role, check if exists, etc.
                var successEmbedBuilder = EmbedBuilderFactoryProvider.Admin.CreateUnmuteSuccess(user.Username);
                return EmbedResult(successEmbedBuilder);
            }

            var failureEmbedBuilder = EmbedBuilderFactoryProvider.Admin.CreateUnmuteFailure();
            return EmbedResult(failureEmbedBuilder);
        }

        [Command("ban"), Alias("unban", "kick", "mute", "unmute")]
        [RequireUserPermission(GuildPermission.BanMembers | GuildPermission.KickMembers | GuildPermission.MuteMembers)]
        public Task<RuntimeResult> UserNotFoundAsync([Remainder] string _)
        {
            var embedBuilder = EmbedBuilderFactoryProvider.Admin.CreateUserNotValidFailure();
            return Task.FromResult(EmbedResult(embedBuilder));
        }
    }
}
