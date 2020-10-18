using Injhinuity.Client.Core.Validation.Enums;

namespace Injhinuity.Client.Core.Validation.Entities
{
    public interface IValidationResult
    {
        ValidationCode ValidationCode { get; }
        string? Message { get; }
    }

    public record ValidationResult(ValidationCode ValidationCode, string? Message = null) : IValidationResult {}
}
