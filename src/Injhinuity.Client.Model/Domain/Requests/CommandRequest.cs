namespace Injhinuity.Client.Model.Domain.Requests
{
    public class CommandRequest : IRequest
    {
        public string Name { get; set; }
        public string Body { get; set; }

        public CommandRequest(string name, string body)
        {
            Name = name;
            Body = body;
        }
    }
}
