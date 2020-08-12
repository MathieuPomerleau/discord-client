using Injhinuity.Client.Core.Validation.Entities;
using Injhinuity.Client.Core.Validation.Entities.Resources.Interfaces;
using Injhinuity.Client.Core.Validation.Validators.SubValidators;

namespace Injhinuity.Client.Core.Validation.Validators
{
    public interface IRootValidator
    {
        ILinkedValidator Root { get; set; }

        IValidationResult Validate(IValidationResource resource) =>
            Root.Validate(resource);

        ILinkedValidator AddRoot(ILinkedValidator root)
        {
            Root = root;
            return Root;
        }
    }
}
