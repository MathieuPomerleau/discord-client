using System.Net;
using System.Threading.Tasks;
using Discord;
using Injhinuity.Client.Enums;
using Injhinuity.Client.Model.Domain;
using Injhinuity.Client.Model.Domain.Responses;
using Injhinuity.Client.Services.Api;
using Injhinuity.Client.Services.Factories;
using Injhinuity.Client.Services.Requesters;

namespace Injhinuity.Client.Discord.Services
{
    public interface IInitialStartupHandler
    {
        Task ExecuteColdStartupAsync();
        Task ExecuteWarmStartupAsync();
    }

    public class InitialStartupHandler : IInitialStartupHandler
    {
        private readonly IInjhinuityDiscordClient _discordClient;
        private readonly IReactionRoleEmbedService _reactionRoleEmbedService;
        private readonly IGuildRequester _guildRequester;
        private readonly IGuildBundleFactory _guildBundleFactory;
        private readonly IRoleRequester _roleRequester;
        private readonly IRoleBundleFactory _roleBundleFactory;
        private readonly IApiReponseDeserializer _deserializer;

        private bool _hasColdStartupRun;

        public InitialStartupHandler(IInjhinuityDiscordClient discordClient, IReactionRoleEmbedService reactionRoleEmbedService,
            IGuildRequester guildRequester, IGuildBundleFactory guildBundleFactory, IRoleRequester roleRequester, IRoleBundleFactory roleBundleFactory,
            IApiReponseDeserializer deserializer)
        {
            _discordClient = discordClient;
            _reactionRoleEmbedService = reactionRoleEmbedService;
            _guildRequester = guildRequester;
            _guildBundleFactory = guildBundleFactory;
            _roleRequester = roleRequester;
            _roleBundleFactory = roleBundleFactory;
            _deserializer = deserializer;
        }

        public async Task ExecuteColdStartupAsync()
        {
            if (_hasColdStartupRun)
                return;

            foreach (var guild in _discordClient.Guilds)
            {
                var id = guild.Id.ToString();

                var getBundle = _guildBundleFactory.Create(id);
                var getResponse = await _guildRequester.ExecuteAsync(ApiAction.Get, getBundle);

                if (getResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    var createBundle = _guildBundleFactory.CreateDefault(guild.Id.ToString());
                    var createResponse = await _guildRequester.ExecuteAsync(ApiAction.Post, createBundle);

                    if (!createResponse.IsSuccessStatusCode)
                    {
                        // Send error, response invalid
                    }
                }
                else if (!getResponse.IsSuccessStatusCode)
                {
                    // Send error, response invalid
                }
            }

            _hasColdStartupRun = true;
        }

        public async Task ExecuteWarmStartupAsync()
        {
            foreach (var guild in _discordClient.Guilds)
            {
                var guildBundle = _guildBundleFactory.Create(guild.Id.ToString());
                var guildResponse = await _guildRequester.ExecuteAsync(ApiAction.Get, guildBundle);

                if (!guildResponse.IsSuccessStatusCode)
                {
                    // Send error, response invalid
                }

                var domainGuild = await _deserializer.StrictDeserializeAndAdaptAsync<GuildResponse, Guild>(guildResponse);
                if (string.IsNullOrWhiteSpace(domainGuild.RoleSettings.ReactionRoleChannelId) ||
                    string.IsNullOrWhiteSpace(domainGuild.RoleSettings.ReactionRoleMessageId))
                    return;

                var roleChannel = await guild.GetTextChannelAsync(ulong.Parse(domainGuild.RoleSettings.ReactionRoleChannelId));
                if (roleChannel is not null && await roleChannel.GetMessageAsync(ulong.Parse(domainGuild.RoleSettings.ReactionRoleMessageId)) is IUserMessage message)
                {
                    var roleBundle = _roleBundleFactory.Create(guild.Id.ToString());
                    var roleResponse = await _roleRequester.ExecuteAsync(ApiAction.GetAll, roleBundle);

                    if (!roleResponse.IsSuccessStatusCode)
                    {
                        // Send error, response invalid
                    }

                    var roles = await _deserializer.StrictDeserializeAndAdaptEnumerableAsync<RoleResponse, Role>(roleResponse);
                    await _reactionRoleEmbedService.InitializeFromMessageAsync(guild, message, roles);
                }
                else
                {
                    // Send error, maybe reset role cause we're supposed to have a message (maybe channel/message was deleted)
                }
            }
        }
    }
}
