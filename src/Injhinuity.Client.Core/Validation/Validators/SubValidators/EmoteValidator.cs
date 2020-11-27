using System;
using Injhinuity.Client.Core.Resources;
using Injhinuity.Client.Core.Validation.Builders;
using Injhinuity.Client.Core.Validation.Entities;
using Injhinuity.Client.Core.Validation.Entities.Resources.Interfaces;

namespace Injhinuity.Client.Core.Validation.Validators.SubValidators
{
    public class EmoteValidator : LinkedValidator
    {
        private readonly Func<IEmoteResource, bool> _emoteParseFunction;

        public EmoteValidator(IValidationResultBuilder validationResultBuilder, Func<IEmoteResource, bool> emoteParseFunction)
            : base(validationResultBuilder)
        {
            _emoteParseFunction = emoteParseFunction;
        }

        public override IValidationResult Validate(IValidationResource resource)
        {
            if (resource is IEmoteResource emote)
            {
                if (IsNullOrEmpty(emote))
                    return ValidationError(ValidationResources.EmoteEmpty);

                if (!_emoteParseFunction(emote))
                    return ValidationError(ValidationResources.EmoteInvalidFormat);

                return Next?.Validate(resource) ?? Ok();
            }

            return ParseError();
        }

        private bool IsNullOrEmpty(IEmoteResource resource) =>
            string.IsNullOrEmpty(resource.Emote);
    }
}
