using MeterSystem.Shared.Models;

namespace MeterSystem.Api.Validation;

internal class MeterReadingsRequestValidator : IMeterReadingsRequestValidator
{
    public ValidationResult Validate(MeterData request)
    {
        List<string> errors = [];

        if (request is null)
        {
            errors.Add("Request body is required.");

            return new ValidationResult(false, errors);
        }

        if (request.MeterNumber <= 0)
        {
            errors.Add("meter_number must be a positive integer.");
        }

        if (request.Readings is null || request.Readings.Count == 0)
        {
            errors.Add("readings must contain at least one entry.");

            return new ValidationResult(false, errors);
        }

        foreach (KeyValuePair<DateTime, double> kv in request.Readings)
        {
            if (kv.Key == default)
            {
                errors.Add("reading timestamp must be a valid DateTime.");
                break;
            }

            if (double.IsNaN(kv.Value) || double.IsInfinity(kv.Value))
            {
                errors.Add($"reading value for '{kv.Key:o}' is not a valid number.");
            }
        }

        return new ValidationResult(errors.Count == 0, errors);
    }
}
