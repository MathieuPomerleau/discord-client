using Injhinuity.Client.Core.Validation.Entities.Resources.Interfaces;

namespace Injhinuity.Client.Core.Validation.Entities.Resources
{
    public record CommandResource(string? Name, string? Body) : IValidationResource, INameResource, IBodyResource {}
}
