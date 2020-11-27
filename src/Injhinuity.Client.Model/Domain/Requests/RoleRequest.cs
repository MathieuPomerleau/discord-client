namespace Injhinuity.Client.Model.Domain.Requests
{
    public record RoleRequest(string Id, string Name, string EmoteString) : IRequest;
}
