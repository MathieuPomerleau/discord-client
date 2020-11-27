namespace Injhinuity.Client.Model.Domain.Requests
{
    public record RoleGuildSettingsRequest(string ReactionRoleChannelId, string ReactionRoleMessageId, string MuteRoleId) : IRequest
    {
        public static RoleGuildSettingsRequest Default() => new RoleGuildSettingsRequest("", "", "");
    }
}
