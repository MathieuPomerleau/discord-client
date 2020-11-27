namespace Injhinuity.Client.Model.Domain.Requests
{
    public record GuildRequest(string Id, RoleGuildSettingsRequest RoleSettings) : IRequest;
}
