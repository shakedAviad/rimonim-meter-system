using MeterSystem.Shared.Models;

namespace MeterSystem.Api.Validation;

internal interface IMeterReadingsRequestValidator
{
    ValidationResult Validate(MeterData request);
}
