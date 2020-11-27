namespace Injhinuity.Client.Model.Domain.Requests
{
    public record CommandRequest(string Name, string Body) : IRequest;
}
