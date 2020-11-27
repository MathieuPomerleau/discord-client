namespace Injhinuity.Client.Model.Domain.Requests.Bundles
{
    public record CommandRequestBundle(string GuildId, CommandRequest? Request = null);
}
