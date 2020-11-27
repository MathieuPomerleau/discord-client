using Discord;
using Injhinuity.Client.Model.Domain.Requests;
using Injhinuity.Client.Model.Domain.Requests.Bundles;

namespace Injhinuity.Client.Services.Factories
{
    public interface IRoleBundleFactory
    {
        RoleRequestBundle Create(string guildId);
        RoleRequestBundle Create(string guildId, string roleId, string roleName, string emote = "");
    }

    public class RoleBundleFactory : IRoleBundleFactory
    {
        public RoleRequestBundle Create(string guildId) =>
            new RoleRequestBundle(guildId);

        public RoleRequestBundle Create(string guildId, string roleId, string roleName, string emote = "") =>
            new RoleRequestBundle(guildId, new RoleRequest(roleId, roleName, emote));
    }
}
