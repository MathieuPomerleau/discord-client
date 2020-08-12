using Injhinuity.Client.Core.Resources;
using Injhinuity.Client.Core.Validation.Builders;
using Injhinuity.Client.Core.Validation.Entities;
using Injhinuity.Client.Core.Validation.Entities.Resources.Interfaces;
using Injhinuity.Client.Core.Validation.Enums;

namespace Injhinuity.Client.Core.Validation.Validators.SubValidators
{
    public interface ILinkedValidator
    {
        ILinkedValidator? Next { get; set; }

        IValidationResult Validate(IValidationResource resource);

        ILinkedValidator AddNext(ILinkedValidator next)
        {
            Next = next;
            return Next;
        }
    }

    public abstract class LinkedValidator : ILinkedValidator
    {
        private readonly IValidationResultBuilder _validationResultBuilder;

        public ILinkedValidator? Next { get; set; }

        protected LinkedValidator(IValidationResultBuilder validationResultBuilder)
        {
            _validationResultBuilder = validationResultBuilder;
        }

        public abstract IValidationResult Validate(IValidationResource resource);

        protected IValidationResult Ok() =>
            _validationResultBuilder.Create()
                .WithCode(ValidationCode.Ok)
                .Build();

        protected IValidationResult ParseError() =>
            _validationResultBuilder.Create()
                .WithCode(ValidationCode.ParseError)
                .WithMessage(ValidationResources.ParseError)
                .Build();

        protected IValidationResult ValidationError(string message) =>
            _validationResultBuilder.Create()
                .WithCode(ValidationCode.ValidationError)
                .WithMessage(message)
                .Build();

        protected IValidationResult ValidationError(string message, params (string, string)[] replacingValues) =>
            _validationResultBuilder.Create()
                .WithCode(ValidationCode.ValidationError)
                .WithMessage(message)
                .WithReplacingValues(replacingValues)
                .Build();
    }
}
