#nullable disable
using Injhinuity.Client.Core.Validation.Validators.SubValidators;

namespace Injhinuity.Client.Core.Validation.Validators
{
    public interface ICommandValidator : IRootValidator
    {
    }

    public class CommandValidator : ICommandValidator
    {
        public ILinkedValidator Root { get; set; }
    }
}
