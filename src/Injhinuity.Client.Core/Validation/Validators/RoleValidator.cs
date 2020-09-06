#nullable disable
using Injhinuity.Client.Core.Validation.Validators.SubValidators;

namespace Injhinuity.Client.Core.Validation.Validators
{
    public interface IRoleValidator : IRootValidator
    {
    }

    public class RoleValidator : IRoleValidator
    {
        public ILinkedValidator Root { get; set; }
    }
}
