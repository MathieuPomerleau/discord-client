using Discord;
using Injhinuity.Client.Model.Domain.Requests;
using Injhinuity.Client.Model.Domain.Requests.Bundles;

namespace Injhinuity.Client.Services.Factories
{
    public interface IRoleBundleFactory
    {
        RoleRequestBundle Create(string guildId);
        RoleRequestBundle Create(string guildId, IRole role);
    }

    public class RoleBundleFactory : IRoleBundleFactory
    {
        public RoleRequestBundle Create(string guildId) =>
            new RoleRequestBundle(guildId);

        public RoleRequestBundle Create(string guildId, IRole role) =>
            new RoleRequestBundle(guildId, new RoleRequest(role.Id.ToString(), role.Name));
    }
}
