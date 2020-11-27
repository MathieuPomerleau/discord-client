using Injhinuity.Client.Core.Resources;
using Injhinuity.Client.Core.Validation.Builders;
using Injhinuity.Client.Core.Validation.Entities;
using Injhinuity.Client.Core.Validation.Entities.Resources.Interfaces;

namespace Injhinuity.Client.Core.Validation.Validators.SubValidators
{
    public class NameValidator : LinkedValidator
    {
        private readonly long _maximumLength;

        public NameValidator(IValidationResultBuilder validationResultBuilder, long maximumLength)
            : base(validationResultBuilder)
        {
            _maximumLength = maximumLength;
        }

        public override IValidationResult Validate(IValidationResource resource)
        {
            if (resource is INameResource name) {
                if (IsNullOrEmpty(name))
                    return ValidationError(ValidationResources.NameEmpty);

                if (IsLengthInvalid(name))
                    return ValidationError(ValidationResources.NameTooLong, (ValidationResources.LengthTag, _maximumLength.ToString()));

                return Next?.Validate(resource) ?? Ok();
            } 

            return ParseError();
        }

        private bool IsNullOrEmpty(INameResource resource) =>
            string.IsNullOrEmpty(resource.Name);

        private bool IsLengthInvalid(INameResource resource) =>
            resource.Name!.Length > _maximumLength;
    }
}
