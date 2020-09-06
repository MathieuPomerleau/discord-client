using Injhinuity.Client.Core.Validation.Entities.Resources.Interfaces;

namespace Injhinuity.Client.Core.Validation.Entities.Resources
{
    public class RoleResource : IValidationResource, INameResource
    {
        public string? Name { get; set; }
        
        public RoleResource(string? name)
        {
            Name = name;
        }
    }
}
