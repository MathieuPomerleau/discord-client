namespace Injhinuity.Client.Model.Domain.Requests
{
    public class RoleRequest : IRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public RoleRequest(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
