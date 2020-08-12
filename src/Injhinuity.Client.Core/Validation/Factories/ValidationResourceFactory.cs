using Injhinuity.Client.Core.Validation.Entities.Resources;
using Injhinuity.Client.Core.Validation.Entities.Resources.Interfaces;

namespace Injhinuity.Client.Core.Validation.Factories
{
    public interface IValidationResourceFactory
    {
        public IValidationResource CreateCommand(string name, string body);
    }

    public class ValidationResourceFactory : IValidationResourceFactory
    {
        public IValidationResource CreateCommand(string name, string body) =>
            new CommandResource(name, body);
    }
}
