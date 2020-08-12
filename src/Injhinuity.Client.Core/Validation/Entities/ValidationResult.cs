using Injhinuity.Client.Core.Validation.Enums;

namespace Injhinuity.Client.Core.Validation.Entities
{
    public interface IValidationResult
    {
        ValidationCode ValidationCode { get; set; }
        string? Message { get; set; }
    }

    public class ValidationResult : IValidationResult
    {
        public ValidationCode ValidationCode { get; set; }
        public string? Message { get; set; }

        public ValidationResult(ValidationCode validationCode, string? message = null)
        {
            ValidationCode = validationCode;
            Message = message;
        }
    }
}
