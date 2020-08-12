using Injhinuity.Client.Core.Resources;
using Injhinuity.Client.Core.Validation.Builders;
using Injhinuity.Client.Core.Validation.Entities;
using Injhinuity.Client.Core.Validation.Entities.Resources.Interfaces;

namespace Injhinuity.Client.Core.Validation.Validators.SubValidators
{
    public class BodyValidator : LinkedValidator
    {
        private readonly long _maximumLength = 0;

        public BodyValidator(IValidationResultBuilder validationResultBuilder, long maximumLength)
            : base(validationResultBuilder)
        {
            _maximumLength = maximumLength;
        }

        public override IValidationResult Validate(IValidationResource resource)
        {
            if (resource is IBodyResource body)
            {
                if (IsNullOrEmpty(body))
                    return ValidationError(ValidationResources.CommandBodyEmpty);

                if (IsLengthInvalid(body))
                    return ValidationError(ValidationResources.CommandBodyTooLong, (ValidationResources.LengthTag, _maximumLength.ToString()));

                return Next?.Validate(resource) ?? Ok();
            }

            return ParseError();
        }

        private bool IsNullOrEmpty(IBodyResource resource) =>
            string.IsNullOrEmpty(resource.Body);

        private bool IsLengthInvalid(IBodyResource resource) =>
            resource.Body.Length > _maximumLength;
    }
}
