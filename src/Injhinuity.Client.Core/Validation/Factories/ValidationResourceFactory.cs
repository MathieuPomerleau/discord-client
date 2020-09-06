using Injhinuity.Client.Core.Validation.Entities.Resources;
using Injhinuity.Client.Core.Validation.Entities.Resources.Interfaces;

namespace Injhinuity.Client.Core.Validation.Factories
{
    public interface IValidationResourceFactory
    {
        IValidationResource CreateCommand(string? name, string? body);
        IValidationResource CreateRole(string? name);
    }

    public class ValidationResourceFactory : IValidationResourceFactory
    {
        public IValidationResource CreateCommand(string? name, string? body) =>
            new CommandResource(name, body);

        public IValidationResource CreateRole(string? name) =>
            new RoleResource(name);
    }
}
