using Injhinuity.Client.Core.Validation.Entities.Resources;
using Injhinuity.Client.Core.Validation.Entities.Resources.Interfaces;

namespace Injhinuity.Client.Core.Validation.Factories
{
    public interface IValidationResourceFactory
    {
        IValidationResource CreateCommand(string? name, string? body);
        IValidationResource CreateRole(string? name, string? emote = null);
    }

    public class ValidationResourceFactory : IValidationResourceFactory
    {
        public IValidationResource CreateCommand(string? name, string? body) =>
            new CommandResource(name, body);

        public IValidationResource CreateRole(string? name, string? emote = null) =>
            new RoleResource(name, emote);
    }
}
