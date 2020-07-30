namespace Injhinuity.Client.Model.Domain.Requests.Bundles
{
    public class CommandRequestBundle
    {
        public string GuildId { get; set; }
        public CommandRequest Request { get; set; }

        public CommandRequestBundle(string guildId, CommandRequest request = null)
        {
            GuildId = guildId;
            Request = request;
        }
    }
}
