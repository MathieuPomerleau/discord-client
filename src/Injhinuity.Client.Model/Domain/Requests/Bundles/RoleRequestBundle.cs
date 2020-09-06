namespace Injhinuity.Client.Model.Domain.Requests.Bundles
{
    public class RoleRequestBundle
    {
        public string GuildId { get; set; }
        public RoleRequest Request { get; set; }

        public RoleRequestBundle(string guildId, RoleRequest request = null)
        {
            GuildId = guildId;
            Request = request;
        }
    }
}
