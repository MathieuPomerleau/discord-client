using Injhinuity.Client.Core.Validation.Entities.Resources.Interfaces;

namespace Injhinuity.Client.Core.Validation.Entities.Resources
{
    public class CommandResource : IValidationResource, INameResource, IBodyResource
    {
        public string Name { get; set; }
        public string Body { get; set; }

        public CommandResource(string name, string body)
        {
            Name = name;
            Body = body;
        }
    }
}
