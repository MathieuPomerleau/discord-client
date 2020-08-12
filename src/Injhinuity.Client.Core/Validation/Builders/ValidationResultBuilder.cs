using Injhinuity.Client.Core.Validation.Entities;
using Injhinuity.Client.Core.Validation.Enums;
using System.Collections.Generic;

namespace Injhinuity.Client.Core.Validation.Builders
{
    public interface IValidationResultBuilder
    {
        IValidationResultBuilder Create();

        IValidationResultBuilder WithCode(ValidationCode code);
        IValidationResultBuilder WithMessage(string messageCode);
        IValidationResultBuilder WithReplacingValue(string tag, string value);
        IValidationResultBuilder WithReplacingValues((string, string)[] replacingValues);

        IValidationResult Build();
    }

    public class ValidationResultBuilder : IValidationResultBuilder
    {
        private ValidationCode _validationCode = ValidationCode.Ok;
        private string? _message;
        private Dictionary<string, string> _replaceValues = new Dictionary<string, string>();

        public IValidationResultBuilder Create()
        {
            _validationCode = ValidationCode.Ok;
            _message = null;
            _replaceValues = new Dictionary<string, string>();

            return this;
        }

        public IValidationResultBuilder WithCode(ValidationCode code)
        {
            _validationCode = code;
            return this;
        }

        public IValidationResultBuilder WithMessage(string message)
        {
            _message = message;
            return this;
        }

        public IValidationResultBuilder WithReplacingValue(string tag, string value)
        {
            _replaceValues.Add(tag, value);
            return this;
        }

        public IValidationResultBuilder WithReplacingValues((string, string)[] replacingValues)
        {
            foreach (var (tag, value) in replacingValues)
                WithReplacingValue(tag, value);

            return this;
        }

        public IValidationResult Build() =>
            new ValidationResult(_validationCode, GetFormattedMessage());

        private string? GetFormattedMessage()
        {
            foreach (var keyPair in _replaceValues)
                _message = _message?.Replace(keyPair.Key, keyPair.Value);

            return _message;
        }
    }
}
