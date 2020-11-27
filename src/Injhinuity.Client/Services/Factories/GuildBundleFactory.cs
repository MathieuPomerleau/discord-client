using Injhinuity.Client.Model.Domain.Requests;
using Injhinuity.Client.Model.Domain.Requests.Bundles;

namespace Injhinuity.Client.Services.Factories
{
    public interface IGuildBundleFactory
    {
        GuildRequestBundle Create(string guildId);
        GuildRequestBundle Create(string guildId, RoleGuildSettingsRequest roleSettings);
        GuildRequestBundle CreateDefault(string guildId);
    }

    public class GuildBundleFactory : IGuildBundleFactory
    {
        public GuildRequestBundle Create(string guildId) =>
            new GuildRequestBundle(guildId);

        public GuildRequestBundle Create(string guildId, RoleGuildSettingsRequest roleSettings) =>
            new GuildRequestBundle(guildId, new GuildRequest(guildId, roleSettings));

        public GuildRequestBundle CreateDefault(string guildId) =>
            new GuildRequestBundle(guildId, new GuildRequest(guildId, RoleGuildSettingsRequest.Default()));
    }
}
