namespace MeterSystem.Api.Validation;

internal record ValidationResult(bool IsValid, IReadOnlyList<string> Errors);
