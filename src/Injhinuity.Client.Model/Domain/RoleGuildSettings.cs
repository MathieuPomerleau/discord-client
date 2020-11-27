namespace Injhinuity.Client.Model.Domain
{
    public record RoleGuildSettings(string ReactionRoleChannelId, string ReactionRoleMessageId, string MuteRoleId)
    {
        public bool IsReactionRoleSetup() =>
            !string.IsNullOrWhiteSpace(ReactionRoleChannelId) &&
            !string.IsNullOrWhiteSpace(ReactionRoleMessageId);
    }
}
